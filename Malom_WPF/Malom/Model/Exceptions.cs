using System;

namespace Malom.Model
{
    public class IllegalMoveException : Exception
    {
        public IllegalMoveException() { }
    }
    public class CannotMoveException : Exception
    {
        public CannotMoveException() { }
    }
    public class InvalidAttackException : Exception
    {
        public InvalidAttackException() { }
    }
    public class BadMarkException : Exception
    {
        public BadMarkException() { }
    }
    public class NotEmptyFieldException : Exception
    {
        public NotEmptyFieldException() { }
    }
    public class OutOfRangeException : Exception
    {
        public OutOfRangeException() { }
    }
    public class NaFException : Exception
    {
        public NaFException() { }
    }
}
