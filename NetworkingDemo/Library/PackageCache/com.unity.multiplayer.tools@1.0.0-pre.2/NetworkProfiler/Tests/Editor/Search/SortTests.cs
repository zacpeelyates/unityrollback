using System;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.Multiplayer.Tools.NetworkProfiler.Editor;
using Unity.Multiplayer.Tools.NetworkProfiler.Runtime;

namespace Unity.Multiplayer.Tools.NetworkProfiler.Tests.Editor
{
    class SortTest
    {
        static readonly List<TestRowData> TestSearchDataList = new List<TestRowData>
        {
            // Game objects
            new TestRowData("Bullet"   , "gameobject", received: 12),
            new TestRowData("Bullet(1)", "gameobject", sent: 8),
            new TestRowData("Bullet(2)", "gameobject", sent: 3),
            new TestRowData("Bullet(3)", "gameobject", received: 4),
            new TestRowData("Bullet(4)", "gameobject", sent: 1),
        };

        [Test]
        [TestCase(SortDirection.NameAscending, "Bullet")]
        [TestCase(SortDirection.NameDescending, "Bullet(4)")]
        public void Sort_PrintableRowSorting_Name(SortDirection direction, string expected)
        {
            var list = new List<IRowData>(TestSearchDataList);

            RowDataSorting.Sort(list, direction);

            Assert.AreEqual(expected, list[0].Name);
        }

        [Test]
        [TestCase(SortDirection.BytesReceivedAscending, 0)]
        [TestCase(SortDirection.BytesReceivedDescending, 12)]
        public void Sort_PrintableRowSorting_BytesReceived(SortDirection direction, int expected)
        {
            var list = new List<IRowData>(TestSearchDataList);

            RowDataSorting.Sort(list, direction);

            Assert.AreEqual(expected, list[0].Bytes.Received);
        }

        [Test]
        [TestCase(SortDirection.BytesSentAscending, 0)]
        [TestCase(SortDirection.BytesSentDescending, 8)]
        public void Sort_PrintableRowSorting_BytesSent(SortDirection direction, int expected)
        {
            var list = new List<IRowData>(TestSearchDataList);

            RowDataSorting.Sort(list, direction);

            Assert.AreEqual(expected, list[0].Bytes.Sent);
        }
    }
}