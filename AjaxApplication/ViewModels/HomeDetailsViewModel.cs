using AjaxApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AjaxApplication.ViewModels
{
    public class HomeDetailsViewModel
    {
        public PicUploadModel PicUploadModel { get; set; }
        public string PageTitle { get; set; }
    }
}