using System;
using System.Collections.Generic;

namespace Unity.Multiplayer.Tools.NetworkProfiler.Editor
{
    static class RowDataSorting
    {
        public static void Sort(List<IRowData> data, SortDirection sortDirection)
        {
            data.Sort((a, b) => SortOperation(a, b, sortDirection));
        }

        public static int SortOperation(IRowData left, IRowData right, SortDirection sortDirection) => sortDirection switch
        {
            SortDirection.NameAscending => NameSortingUp(left, right),
            SortDirection.NameDescending => NameSortingDown(left, right),
            SortDirection.TypeAscending => TypeSortingUp(left, right),
            SortDirection.TypeDescending => TypeSortingDown(left, right),
            SortDirection.BytesSentAscending => BytesSentSortingUp(left, right),
            SortDirection.BytesSentDescending => BytesSentSortingDown(left, right),
            SortDirection.BytesReceivedAscending => BytesReceivedSortingUp(left, right),
            SortDirection.BytesReceivedDescending => BytesReceivedSortingDown(left, right),
            _ => throw new ArgumentOutOfRangeException(nameof(sortDirection), sortDirection, null),
        };

        static int NameSortingUp(IRowData left, IRowData right)
        {
            return StringSortingUp(left.Name, right.Name);
        }

        static int NameSortingDown(IRowData left, IRowData right)
        {
            return -NameSortingUp(left, right);
        }

        static int TypeSortingUp(IRowData left, IRowData right)
        {
            return StringSortingUp(left.TypeDisplayName, right.TypeDisplayName);
        }

        static int TypeSortingDown(IRowData left, IRowData right)
        {
            return -TypeSortingUp(left, right);
        }

        static int BytesSentSortingUp(IRowData left, IRowData right)
        {
            return left.Bytes.Sent.CompareTo(right.Bytes.Sent);
        }

        static int BytesSentSortingDown(IRowData left, IRowData right)
        {
            return -BytesSentSortingUp(left, right);
        }

        static int BytesReceivedSortingUp(IRowData left, IRowData right)
        {
            return (left.Bytes.Received).CompareTo(right.Bytes.Received);
        }

        static int BytesReceivedSortingDown(IRowData left, IRowData right)
        {
            return -BytesReceivedSortingUp(left, right);
        }

        static int StringSortingUp(string left, string right)
        {
            if (string.IsNullOrWhiteSpace(left) && string.IsNullOrWhiteSpace(right))
            {
                return 0;
            }

            if (string.IsNullOrWhiteSpace(left))
            {
                return -1;
            }

            return string.IsNullOrWhiteSpace(right)
                ? 1
                : string.Compare(left, right, StringComparison.Ordinal);
        }
    }
}