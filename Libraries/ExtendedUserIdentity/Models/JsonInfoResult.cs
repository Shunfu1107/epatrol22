using System;

namespace AdminPortalV8.Libraries.ExtendedUserIdentity.Models
{
    [Serializable]
    public class JsonInfoResult
    {
        private bool _success;
        private string? _code;
        private Exception? _exception;
        private bool _isHtml;
        private object? _data;

        public JsonInfoResult()
        {
            Success = true;
        }

        public bool Success
        {
            get { return _success; }
            set
            {
                _success = value;
            }
        }

        public string? Code
        {
            get { return _code; }
            set
            {
                _code = value;
            }
        }

        public Exception? Exception
        {
            get { return _exception; }
            set
            {
                _exception = value;
            }
        }

        public bool IsHtml
        {
            get { return _isHtml; }
            set
            {
                _isHtml = value;
            }
        }

        public object? Data
        {
            get { return _data; }
            set
            {
                _data = value;
            }
        }

        public object ToJson()
        {
            return new
            {
                Success = Success,
                Code = Code,
                Exception = Exception,
                Message = Exception != null ? Exception.Message : string.Empty,
                Data = Data,
                IsHtml = IsHtml
            };
        }
    }
}