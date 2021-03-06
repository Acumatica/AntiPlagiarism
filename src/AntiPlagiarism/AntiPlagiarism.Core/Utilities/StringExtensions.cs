﻿using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace AntiPlagiarism.Core.Utilities
{
    public static class StringExtensions
	{
		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsNullOrWhiteSpace(this string str) => string.IsNullOrWhiteSpace(str);

		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsNullOrEmpty(this string str) => string.IsNullOrEmpty(str);

		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsEmpty(this string source)
		{
			source.ThrowOnNull(nameof(source));

			return source.Length == 0;
		}
	}
}
