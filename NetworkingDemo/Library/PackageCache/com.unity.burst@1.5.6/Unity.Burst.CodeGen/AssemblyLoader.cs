using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
#if BURST_COMPILER_SHARED
using Burst.Compiler.IL;
using Burst.Compiler.IL.DebugInfo;
using Burst.Compiler.IL.Diagnostics;
using Burst.Compiler.IL.Helpers;
#endif
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Pdb;

namespace zzzUnity.Burst.CodeGen
{
    /// <summary>
    /// Provides an assembly loader with caching depending on the LastWriteTime of the assembly file.
    /// </summary>
    /// <remarks>
    /// This class is not thread safe. It needs to be protected outside.
    /// </remarks>
#if BURST_COMPILER_SHARED
    public
#else
    internal
#endif
    class AssemblyLoader : BaseAssemblyResolver
    {
        private readonly Dictionary<string, CacheAssemblyEntry> _nameToEntry;
        private readonly Dictionary<string, CacheAssemblyEntry> _fileToEntry;

#if BURST_COMPILER_SHARED
        public readonly PortablePdbCache PdbCacheReference;

        public AssemblyLoader(PortablePdbCache instance)
#else
        public AssemblyLoader()
#endif
        {
            _fileToEntry = new Dictionary<string, CacheAssemblyEntry>(StringComparer.Ordinal);
            _nameToEntry = new Dictionary<string, CacheAssemblyEntry>(StringComparer.Ordinal);
#if BURST_COMPILER_SHARED
            PdbCacheReference = instance ?? throw new ArgumentException("instance must point to a valid PortablePdbCache instance");
#endif

            // We remove all setup by Cecil by default (it adds '.' and 'bin')
            ClearSearchDirectories();

            LoadDebugSymbols = false;       // We don't bother loading the symbols by default now, since we use SRM to handle symbols in a more thread safe manner
                                            // this is to maintain compatibility with the patch-assemblies path (see BclApp.cs), used by dots runtime
        }

        /// <summary>
        /// Check if assemblies are dirty. By default, this feature is disabled as it is slowing down a lot with kernel context switch to test the filesystem.
        /// </summary>
        public bool CheckAssemblyDirty { get; set; }

        public bool IsDebugging { get; set; }

        public bool LoadDebugSymbols { get; set; }

        public Action<LogMessageType, string> OnLogDebug { get; set; }

        internal Action<AssemblyNameReferenceAndPath> OnResolve { get; set; }

        internal Action<AssemblyNameReference, Action<LogMessageType, string>> OnDirty { get; set; }

        public void Clear()
        {
            foreach (var entry in this._nameToEntry.Values)
                entry.Definition.Dispose();
            _nameToEntry.Clear();
            _fileToEntry.Clear();
        }

        public void ClearSearchDirectories()
        {
            foreach (var dir in GetSearchDirectories())
            {
                RemoveSearchDirectory(dir);
            }
        }

        public AssemblyDefinition LoadFromStream(Stream peStream, Stream pdbStream = null, ISymbolReaderProvider customSymbolReader=null)
        {
            peStream.Position = 0;
            if (pdbStream != null)
            {
                pdbStream.Position = 0;
            }
            var readerParameters = CreateReaderParameters();
            if (customSymbolReader != null)
            {
                readerParameters.ReadSymbols = true;
                readerParameters.SymbolReaderProvider = customSymbolReader;
            }
            readerParameters.ReadingMode = ReadingMode.Deferred;
            try
            {
                readerParameters.SymbolStream = pdbStream;
                return AssemblyDefinition.ReadAssembly(peStream, readerParameters);
            }
            catch
            {
                readerParameters.ReadSymbols = false;
                readerParameters.SymbolStream = null;
                peStream.Position = 0;
                if (pdbStream != null)
                {
                    pdbStream.Position = 0;
                }
                return AssemblyDefinition.ReadAssembly(peStream, readerParameters);
            }
        }

        public override AssemblyDefinition Resolve(AssemblyNameReference name)
        {
            CacheAssemblyEntry cacheEntry;

            if (this._nameToEntry.TryGetValue(name.FullName, out cacheEntry))
            {
                if (!IsCacheEntryDirtyAndNotify(cacheEntry))
                {
                    OnResolve?.Invoke(new AssemblyNameReferenceAndPath(name, cacheEntry.FilePath));
                    return cacheEntry.Definition;
                }

                RemoveEntryFromCache(cacheEntry.Name, cacheEntry);
            }

            var readerParameters = CreateReaderParameters();
            readerParameters.ReadingMode = ReadingMode.Deferred;
            AssemblyDefinition assemblyDefinition;

            try
            {
                assemblyDefinition = this.Resolve(name, readerParameters);
            }
            catch
            {
                if (readerParameters.ReadSymbols == true)
                {
                    // Attempt to load without symbols
                    readerParameters.ReadSymbols = false;
                    assemblyDefinition = this.Resolve(name, readerParameters);
                }
                else
                {
                    throw;
                }
            }

            RegisterAssembly(name, assemblyDefinition);
            OnResolve?.Invoke(new AssemblyNameReferenceAndPath(name, _nameToEntry[name.FullName].FilePath));
            return assemblyDefinition;
        }

        public bool TryGetFullPath(AssemblyNameReference name, out string fullPath)
        {
            try
            {
                // We don't care about the return value - we just want to ensure
                // that _nameToEntry has the correct cache entry.
                Resolve(name);
            }
            catch (AssemblyResolutionException)
            {
                fullPath = null;
                return false;
            }

            var cacheEntry = _nameToEntry[name.FullName];
            fullPath = cacheEntry.FilePath;
            return true;
        }

        private bool IsCacheEntryDirtyAndNotify(CacheAssemblyEntry entry)
        {
            // By default, we don't check assembly dirtiness as it is requiring a costly kernel context switch with the filesystem
            // and hurting significantly the performance for btests
            // CheckAssemblyDirty is only active for JitCompilerService
            if (!CheckAssemblyDirty) return false;

            if (entry.FileTimeVerified) return false;

            var lastWriteTime = File.GetLastWriteTime(entry.FilePath);
            var isDirty = lastWriteTime > entry.FileTime;
            entry.FileTimeVerified = true;

            if (IsDebugging)
            {
                OnLogDebug?.Invoke(LogMessageType.Debug, $"Checking Assembly file timestamp {entry.FilePath} Cached: `{DateTimeToStringPrecise(entry.FileTime)}` OnDisk: `{DateTimeToStringPrecise(lastWriteTime)}` => {(isDirty?"DIRTY" : "Not dirty")}");
            }

            if (isDirty)
            {
                OnDirty?.Invoke(entry.Definition.Name, OnLogDebug);
                return true;
            }
            return false;
        }

        private static string DateTimeToStringPrecise(DateTime datetime)
        {
            return datetime.ToString("yyyy-MM-dd HH:mm:ss.fff");
        }

        public string DumpCache()
        {
            var builder = new StringBuilder();
            foreach (var cacheAssemblyEntry in _nameToEntry)
            {
                builder.AppendLine(cacheAssemblyEntry.Value.ToString());
            }

            return builder.ToString();
        }

        public void UpdateCache()
        {
            if (!CheckAssemblyDirty) return;

            var keys = _nameToEntry.Keys.ToArray();

            foreach (var key in keys)
            {
                var entry = _nameToEntry[key];
                entry.FileTimeVerified = false;
                if (IsCacheEntryDirtyAndNotify(entry))
                {
                    RemoveEntryFromCache(key, entry);
                }
            }
        }

        private void RemoveEntryFromCache(string entryKey, CacheAssemblyEntry entry)
        {
#if BURST_COMPILER_SHARED
            PdbCacheReference.RemoveEntry(entry.Definition, OnLogDebug);
#endif
            _nameToEntry.Remove(entryKey);
            _fileToEntry.Remove(entry.FilePath);
        }

        /// <summary>
        /// Remove assemblies from the cache that no longer exist on the disk.
        /// </summary>
        public void Prune()
        {
            foreach (var file in _fileToEntry.Keys.ToList())
            {
                if (!File.Exists(file))
                {
                    if (IsDebugging)
                    {
                        OnLogDebug?.Invoke(LogMessageType.Debug, $"Assembly file removed from cache {file}");
                    }

                    var entry = _fileToEntry[file];
                    _fileToEntry.Remove(file);
                    _nameToEntry.Remove(entry.Definition.FullName);
                }
            }
        }

        public new void AddSearchDirectory(string directory)
        {
            if (!GetSearchDirectories().Contains(directory))
            {
                base.AddSearchDirectory(directory);
            }
        }

        /// <summary>
        /// Loads the specified assembly.
        /// </summary>
        /// <param name="assemblyLocation">The assembly location.</param>
        /// <param name="useSymbols">if set to <c>true</c> load and use the pdb symbols.</param>
        /// <exception cref="ArgumentNullException">assemblyLocation</exception>
        /// <returns>The loaded Assembly definition.</returns>
        public AssemblyDefinition LoadFromFile(string assemblyLocation)
        {
            if (assemblyLocation == null) throw new ArgumentNullException(nameof(assemblyLocation));

            // If the file was already loaded, don't try to load it
            CacheAssemblyEntry cacheEntry;
            assemblyLocation = NormalizeFilePath(assemblyLocation);
            if (this._fileToEntry.TryGetValue(assemblyLocation, out cacheEntry))
            {
                if (!IsCacheEntryDirtyAndNotify(cacheEntry))
                {
                    return cacheEntry.Definition;
                }

                RemoveEntryFromCache(cacheEntry.Name, cacheEntry);
            }

            if (assemblyLocation == null) throw new ArgumentNullException(nameof(assemblyLocation));
            var readerParams = CreateReaderParameters();
            AssemblyDefinition assemblyDefinition;
            try
            {
                assemblyDefinition = AssemblyDefinition.ReadAssembly(assemblyLocation, readerParams);
            }
            catch (Exception)
            {
                if (readerParams.ReadSymbols == true)
                {
                    // Attempt to load without symbols
                    readerParams.ReadSymbols = false;
                    assemblyDefinition = AssemblyDefinition.ReadAssembly(assemblyLocation, readerParams);
                }
                else
                {
                    throw;
                }
            }
            // AssemblyDefinition.Load For some reason, assemblyLoader doesn't cache properly
            RegisterAssembly(assemblyDefinition.Name, assemblyDefinition);

            return assemblyDefinition;
        }

        private ReaderParameters CreateReaderParameters()
        {
            var readerParams = new ReaderParameters
            {
                InMemory = true,
                AssemblyResolver = this,
                MetadataResolver =  new CustomMetadataResolver(this),
                ReadSymbols = LoadDebugSymbols     // We no longer use cecil to read symbol information, prefering SRM thread safe methods, so I`m being explicit here in case the default changes
            };

            if (LoadDebugSymbols)
            {
                readerParams.SymbolReaderProvider = new CustomSymbolReaderProvider(null);
            }

            return readerParams;
        }

#if BURST_COMPILER_SHARED
        /// <summary>
        /// Resolves a cecil <see cref="MethodReference"/> from a <see cref="MethodReferenceString"/>
        /// </summary>
        /// <param name="methodReferenceString">A method reference string</param>
        /// <returns>A cecil <see cref="MethodReference"/></returns>
        public MethodReference Resolve(MethodReferenceString methodReferenceString)
        {
            if (methodReferenceString == null) throw new ArgumentNullException(nameof(methodReferenceString));

            var typeReference = Resolve(methodReferenceString.DeclaringType);

            var typeDefinition = typeReference.StrictResolve();

            // Initial capacity 1 because that is overwhelmingly the likely outcome.
            var methods = new List<MethodDefinition>(1);

            foreach (var m in typeDefinition.Methods)
            {
                if (m.Name != methodReferenceString.Name)
                {
                    continue;
                }

                if (m.Parameters.Count != methodReferenceString.ParameterTypes.Count)
                {
                    continue;
                }

                methods.Add(m);
            }

            // We expect to match at least one method
            if (methods.Count == 0)
            {
                throw new InvalidOperationException($"Unable to find the method `{methodReferenceString}` from type `{typeReference}`");
            }

            // If we have more than one match we need to do a more expensive re-evaluation of the methods to figure out which types they use match.
            if (methods.Count > 1)
            {
                MethodDefinition methodToUse = null;

                foreach (var m in methods)
                {
                    var count = m.Parameters.Count;

                    var match = true;

                    for (int i = 0; i < count; i++)
                    {
                        var parameterTypeFullName = methodReferenceString.ParameterTypes[i].ToString(ReferenceStringFormatOptions.None).Replace("+", "/").Replace("[[", "<").Replace("]]", ">");

                        if (m.Parameters[i].ParameterType.FullName != parameterTypeFullName)
                        {
                            match = false;
                            break;
                        }
                    }

                    if (match)
                    {
                        methodToUse = m;
                        break;
                    }
                }

                if (methodToUse == null)
                {
                    throw new Exception($"Unable to resolve the method `{methodReferenceString}` from type `{typeReference}` to a single method from set of {methods.Count} methods");
                }

                methods.Clear();
                methods.Add(methodToUse);
            }

            var method = methods[0];
            var methodReference = new MethodReference(method.Name, method.ReturnType, typeReference);

            foreach (var param in method.Parameters)
            {
                methodReference.Parameters.Add(new ParameterDefinition(param.Name, param.Attributes, param.ParameterType));
            }

            return methodReference;
        }

        public TypeReference Resolve(Type simpleType)
        {
            var simpleTypeRefAsString = new SimpleTypeReferenceString(simpleType.FullName)
            {
                Assembly = AssemblyNameReference.Parse(simpleType.Assembly.FullName)
            };
            return Resolve(simpleTypeRefAsString);
        }

        /// <summary>
        /// Resolves a cecil <see cref="TypeReference"/> from a <see cref="SimpleTypeReferenceString"/>
        /// </summary>
        /// <param name="simpleTypeReferenceString">A type reference string</param>
        /// <returns>A cecil <see cref="TypeReference"/></returns>
        public TypeReference Resolve(SimpleTypeReferenceString simpleTypeReferenceString)
        {
            if (simpleTypeReferenceString == null) throw new ArgumentNullException(nameof(simpleTypeReferenceString));
            var assemblyOfMainType = Resolve(simpleTypeReferenceString.Assembly);

            var subTypes = simpleTypeReferenceString.FullName.Split(new char[] {'+'});
            Guard.Assert(subTypes.Length >= 1);
            var typeReference = (TypeReference)assemblyOfMainType.MainModule.GetType(subTypes[0]);
            if (typeReference == null)
            {
                throw new InvalidOperationException(
                    $"Unable to find type `{simpleTypeReferenceString.FullName}` from assembly `{simpleTypeReferenceString.Assembly}`");
            }

            for (var i = 1; i < subTypes.Length; i++)
            {
                var subType = subTypes[i];
                var definition = typeReference.StrictResolve();
                var nestedDefinition = definition.NestedTypes.FirstOrDefault(nestedType => nestedType.Name == subType);
                if (nestedDefinition == null)
                {
                    throw new InvalidOperationException($"Unable to find nested type `{subType}` from typename `{simpleTypeReferenceString.FullName}` assembly `{simpleTypeReferenceString.Assembly}`");
                }
                typeReference = nestedDefinition;
            }

            var generic = simpleTypeReferenceString as GenericInstanceTypeReferenceString;
            if (generic != null)
            {
                var genericType = new GenericInstanceType(typeReference);
                foreach (var genericArgType in generic.GenericArguments)
                {
                    var simpleGenericArgType = genericArgType as SimpleTypeReferenceString;
                    if (simpleGenericArgType == null)
                    {
                        throw new InvalidOperationException($"Unable to resolve generic argument `{genericArgType}`");
                    }
                    genericType.GenericArguments.Add(Resolve(simpleGenericArgType));
                }
                typeReference = genericType;
            }

            return typeReference;
        }
#endif

        private void RegisterAssembly(AssemblyNameReference name, AssemblyDefinition assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));
            string fullName = name.FullName;
            var filename = GetAssemblyFileName(assembly);
            var entry = new CacheAssemblyEntry(assembly, filename);
            _nameToEntry[fullName] = entry;
            // Duplicate the entry (mscorlib 2.0.0 can be remapped to 4.0.0, so better cache them both)
            _nameToEntry[assembly.Name.FullName] = entry;
            _fileToEntry[filename] = entry;
#if BURST_COMPILER_SHARED
            PdbCacheReference.AddEntry(assembly, OnLogDebug);
#endif
        }

        private string GetAssemblyFileName(AssemblyDefinition assembly)
        {
            string fileName = assembly.MainModule.FileName;
            if (fileName == null)
            {
                throw new InvalidOperationException($"Unable to find original assembly file from {assembly.Name}");
            }
            return NormalizeFilePath(fileName);
        }

        protected override void Dispose(bool disposing)
        {
            Clear();
            base.Dispose(disposing);
        }

        private class CacheAssemblyEntry
        {
            public CacheAssemblyEntry(AssemblyDefinition assemblyDefinition, string filePath)
            {
                Definition = assemblyDefinition;
                FilePath = filePath;
                FileTime = File.GetLastWriteTime(filePath);
                FileTimeVerified = true;
            }

            public string Name => Definition.FullName;

            public readonly AssemblyDefinition Definition;

            public readonly string FilePath;

            public readonly DateTime FileTime;

            public bool FileTimeVerified { get; set; }

            public override string ToString() => $"{Name} => {FilePath} {DateTimeToStringPrecise(FileTime)}";
        }

        private static string NormalizeFilePath(string path)
        {
            return Path.GetFullPath(new Uri(path).LocalPath).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        }

        private class CustomMetadataResolver : MetadataResolver
        {
            public CustomMetadataResolver(IAssemblyResolver assemblyResolver) : base(assemblyResolver)
            {
            }

            public override MethodDefinition Resolve(MethodReference method)
            {
                if (method is MethodDefinition methodDef)
                {
                    return methodDef;
                }

                if (method.GetElementMethod() is MethodDefinition methodDef2)
                {
                    return methodDef2;
                }

                return base.Resolve(method);
            }
        }

        /// <summary>
        /// Custom implementation of <see cref="ISymbolReaderProvider"/> to:
        /// - to load pdb/mdb through a MemoryStream to avoid locking the file on the disk
        /// - catch any exceptions while loading the symbols and report them back
        /// </summary>
        private class CustomSymbolReaderProvider : ISymbolReaderProvider
        {
            private readonly Action<string, Exception> _logException;

            public CustomSymbolReaderProvider(Action<string, Exception> logException)
            {
                _logException = logException;
            }

            public ISymbolReader GetSymbolReader(ModuleDefinition module, string fileName)
            {
                if (string.IsNullOrWhiteSpace(fileName)) return null;

                string pdbFileName = fileName;
                try
                {
                    fileName = NormalizeFilePath(fileName);
                    pdbFileName = GetPdbFileName(fileName);

                    if (File.Exists(pdbFileName))
                    {
                        var pdbStream = ReadToMemoryStream(pdbFileName);
                        if (IsPortablePdb(pdbStream))
                            return new SafeDebugReaderProvider(new PortablePdbReaderProvider().GetSymbolReader(module, pdbStream));

                        return new SafeDebugReaderProvider(new NativePdbReaderProvider().GetSymbolReader(module, pdbStream));
                    }
                }
                catch (Exception ex) when (_logException != null)
                {
                    _logException?.Invoke($"Unable to load symbol `{pdbFileName}`", ex);
                    return null;
                }
                return null;
            }

            private static MemoryStream ReadToMemoryStream(string filename)
            {
                return new MemoryStream(File.ReadAllBytes(filename));
            }

            public ISymbolReader GetSymbolReader(ModuleDefinition module, Stream symbolStream)
            {
                throw new NotSupportedException();
            }

            private static string GetPdbFileName(string assemblyFileName)
            {
                return Path.ChangeExtension(assemblyFileName, ".pdb");
            }

            private static bool IsPortablePdb(Stream stream)
            {
                if (stream.Length < 4L)
                    return false;
                long position = stream.Position;
                try
                {
                    return (int)new BinaryReader(stream).ReadUInt32() == 1112167234;
                }
                finally
                {
                    stream.Position = position;
                }
            }

            /// <summary>
            /// This class is a wrapper around <see cref="ISymbolReader"/> to protect
            /// against failure while trying to read debug information in Mono.Cecil
            /// </summary>
            private class SafeDebugReaderProvider : ISymbolReader
            {
                private readonly ISymbolReader _reader;

                public SafeDebugReaderProvider(ISymbolReader reader)
                {
                    _reader = reader;
                }


                public void Dispose()
                {
                    try
                    {
                        _reader.Dispose();
                    }
                    catch
                    {
                        // ignored
                    }
                }

                public ISymbolWriterProvider GetWriterProvider()
                {
                    // We are not protecting here as we are not suppose to write to PDBs
                    return _reader.GetWriterProvider();
                }

                public bool ProcessDebugHeader(ImageDebugHeader header)
                {
                    try
                    {
                        return _reader.ProcessDebugHeader(header);
                    }
                    catch
                    {
                        // ignored
                    }

                    return false;
                }

                public MethodDebugInformation Read(MethodDefinition method)
                {
                    try
                    {
                        return _reader.Read(method);
                    }
                    catch
                    {
                        // ignored
                    }
                    return null;
                }
            }
        }

#if !BURST_COMPILER_SHARED
        public enum LogMessageType
        {
            Debug,
        }
#endif
    }

    /// <summary>
    /// This class is a container for keeping an assembly reference and path together
    /// </summary>
    internal sealed class AssemblyNameReferenceAndPath
    {
        /// <summary>
        /// Get the assembly name reference
        /// </summary>
        public readonly AssemblyNameReference AssemblyNameReference;
        /// <summary>
        /// Get the full path to the assembly
        /// </summary>
        public readonly string FullPath;

        internal AssemblyNameReferenceAndPath(AssemblyNameReference assemblyNameReference, string fullPath)
        {
            AssemblyNameReference = assemblyNameReference;
            FullPath = fullPath;
        }
    }
}
