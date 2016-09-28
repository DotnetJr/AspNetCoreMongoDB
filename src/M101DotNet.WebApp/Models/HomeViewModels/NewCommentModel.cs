using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace M101DotNet.WebApp.Models.HomeViewModels
{
    public class NewCommentModel
    {
        [HiddenInput(DisplayValue = false)]
        public string PostId { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string Content { get; set; }
    }
}