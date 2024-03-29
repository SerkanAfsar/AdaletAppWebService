﻿using System.Net;

namespace AdaletApp.WEBAPI.Utilities
{
    public class ResponseResult<T> where T : class
    {
        public T Entity { get; set; }
        public List<T> Entities { get; set; }
        public bool IsSuccess { get; set; }
        public bool HasError { get; set; }
        public List<string> ErrorList { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public int TotalCount { get; set; } = 0;
        public int PaginationItemCount { get; set; } = 0;

        public ResponseResult()
        {
            this.Entities = new List<T>();
            this.Entity = null;
            this.IsSuccess = true;
            this.HasError = false;
            this.ErrorList = new List<string>();
            this.StatusCode = HttpStatusCode.OK;
            this.TotalCount = 0;
            this.PaginationItemCount = 0;
        }
    }
}
