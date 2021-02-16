﻿using System.Collections.Generic;
using System.Net;

namespace MongoTutorial.Api.Common
{
    public class ApiError
    {
        public Dictionary<string, IEnumerable<string>> Errors { get; }
        public int Status { get; set; }

        public ApiError(string field, string error, int status)
        {
            Errors = new Dictionary<string, IEnumerable<string>>
            {
                {field, new[] {error}}
            };
            Status = status;
        }
    }
}