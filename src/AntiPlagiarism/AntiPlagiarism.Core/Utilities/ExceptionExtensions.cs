﻿using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace AntiPlagiarism.Core.Utilities
{
	public static class ExceptionExtensions
	{
		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ThrowOnNull<T>(this T obj, string parameter = null, string message = null)
		{
			if (obj != null)
				return;

			throw NewArgumentNullException(parameter, message);
		}

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowOnNullOrWhiteSpace(this string str, string parameter = null, string message = null)
        {
            if (!string.IsNullOrWhiteSpace(str))
                return;

            throw str == null
                ? NewArgumentNullException(parameter, message)
                : NewArgumentException(parameter, message);
        }

        [DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ThrowOnNullOrEmpty(this string str, string parameter = null, string message = null)
		{
			if (!string.IsNullOrEmpty(str))
				return;

			throw str == null
				? NewArgumentNullException(parameter, message)
				: NewArgumentException(parameter, message);
		}

		private static ArgumentNullException NewArgumentNullException(string parameter = null, string message = null)
		{
			return parameter == null
			   ? throw new ArgumentNullException()
			   : message == null
				   ? new ArgumentNullException(parameter)
				   : new ArgumentNullException(parameter, message);
		}

		private static ArgumentException NewArgumentException(string parameter = null, string message = null)
		{
			return parameter == null
			   ? throw new ArgumentNullException()
			   : message == null
				   ? new ArgumentNullException(parameter)
				   : new ArgumentNullException(parameter, message);
		}
	}
}
