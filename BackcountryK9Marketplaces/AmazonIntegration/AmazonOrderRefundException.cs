﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BackcountryK9Marketplaces.AmazonIntegration
{
    public class AmazonOrderRefundException: Exception
    {
        /// <summary>
        /// The collection of results that errored out.
        /// </summary>
        public List<Result> ErrorResults { get; set; }

        public AmazonOrderRefundException(string message)
            : base(message)
        {
            ErrorResults = new List<Result>();
        }


        public class Result
        {
            public string Code { get; set; }
            public string Description { get; set; }
        }
    }
}
