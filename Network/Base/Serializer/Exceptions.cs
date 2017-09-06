using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Network.Serializer
{
    class NSException: Exception
    {
        public NSException(string msg) : base(msg) { }
    }

    class ConfigErrorException : NSException
    {
        public ConfigErrorException(string file)
            : base(string.Format("\"{0}\" is not a valid net stream type config file.", file)) 
        { }
    }

    class UnexistTypeException : NSException
    {
        public UnexistTypeException(string tname)
            : base(string.Format("net stream type({0}) is not exist", tname))
        { }
    }

    class UnexistCommandException : NSException
    {
        public UnexistCommandException(string cmd)
            : base(string.Format("net stream command({0}) is not exist", cmd))
        { }
    }

    class UnexistCommandImplementException : NSException
    {
        public UnexistCommandImplementException(string cmd)
            : base(string.Format("command({0})'s implement is not exist.", cmd))
        { }
    }

    class ErrorTypeException : NSException
    {
        public ErrorTypeException(string tname, object value)
            : base(string.Format("error type value({0}) of {1}", value, tname))
        {            
        }
    }

    class ErrorStreamException : NSException
    {
        public ErrorStreamException()
            : base(string.Format("stream buff length error"))
        {
        }
    }
}
