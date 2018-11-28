using System;
using System.Runtime.Serialization;

namespace Ryu64.Common
{
    public class Exceptions
    {
        public class InvalidOrUnimplementedMemoryMapException : Exception
        {
            public InvalidOrUnimplementedMemoryMapException()
                : base() { }

            public InvalidOrUnimplementedMemoryMapException(string message)
                : base(message) { }

            public InvalidOrUnimplementedMemoryMapException(string format, params object[] args)
                : base(string.Format(format, args)) { }

            public InvalidOrUnimplementedMemoryMapException(string message, Exception innerException)
                : base(message, innerException) { }

            public InvalidOrUnimplementedMemoryMapException(string format, Exception innerException, params object[] args)
                : base(string.Format(format, args), innerException) { }
        }

        public class MemoryProtectionViolation : Exception
        {
            public MemoryProtectionViolation()
                : base() { }

            public MemoryProtectionViolation(string message)
                : base(message) { }

            public MemoryProtectionViolation(string format, params object[] args)
                : base(string.Format(format, args)) { }

            public MemoryProtectionViolation(string message, Exception innerException)
                : base(message, innerException) { }

            public MemoryProtectionViolation(string format, Exception innerException, params object[] args)
                : base(string.Format(format, args), innerException) { }
        }

        public class ProgramBreakPointException : Exception
        {
            public ProgramBreakPointException()
                : base() { }

            public ProgramBreakPointException(string message)
                : base(message) { }

            public ProgramBreakPointException(string format, params object[] args)
                : base(string.Format(format, args)) { }

            public ProgramBreakPointException(string message, Exception innerException)
                : base(message, innerException) { }

            public ProgramBreakPointException(string format, Exception innerException, params object[] args)
                : base(string.Format(format, args), innerException) { }
        }

        public class InvalidOperationException : Exception
        {
            public InvalidOperationException()
                : base() { }

            public InvalidOperationException(string message)
                : base(message) { }

            public InvalidOperationException(string format, params object[] args)
                : base(string.Format(format, args)) { }

            public InvalidOperationException(string message, Exception innerException)
                : base(message, innerException) { }

            public InvalidOperationException(string format, Exception innerException, params object[] args)
                : base(string.Format(format, args), innerException) { }
        }

        public class TLBMissException : Exception
        {
            public TLBMissException()
                : base() { }

            public TLBMissException(string message)
                : base(message) { }

            public TLBMissException(string format, params object[] args)
                : base(string.Format(format, args)) { }

            public TLBMissException(string message, Exception innerException)
                : base(message, innerException) { }

            public TLBMissException(string format, Exception innerException, params object[] args)
                : base(string.Format(format, args), innerException) { }
        }
    }
}
