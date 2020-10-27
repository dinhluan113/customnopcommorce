using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Nop.Core.Domain.Catalog;
using Nop.Services.Common;

namespace Nop.Admin.Extensions
{
    /// <summary>
    /// Parser helper
    /// </summary>
    public static class AttributeParserHelper
    {
        public static string ParseCustomAddressAttributes(this FormCollection form,
            IAddressAttributeParser addressAttributeParser,
            IAddressAttributeService addressAttributeService)
        {
            if (form == null)
                throw new ArgumentNullException("form");

            string attributesXml = "";
            var attributes = addressAttributeService.GetAllAddressAttributes();
            foreach (var attribute in attributes)
            {
                string controlId = string.Format("address_attribute_{0}", attribute.Id);
                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                    case AttributeControlType.RadioList:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!String.IsNullOrEmpty(ctrlAttributes))
                            {
                                int selectedAttributeId = int.Parse(ctrlAttributes);
                                if (selectedAttributeId > 0)
                                    attributesXml = addressAttributeParser.AddAddressAttribute(attributesXml,
                                        attribute, selectedAttributeId.ToString());
                            }
                        }
                        break;
                    case AttributeControlType.Checkboxes:
                        {
                            var cblAttributes = form[controlId];
                            if (!String.IsNullOrEmpty(cblAttributes))
                            {
                                foreach (var item in cblAttributes.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                                {
                                    int selectedAttributeId = int.Parse(item);
                                    if (selectedAttributeId > 0)
                                        attributesXml = addressAttributeParser.AddAddressAttribute(attributesXml,
                                            attribute, selectedAttributeId.ToString());
                                }
                            }
                        }
                        break;
                    case AttributeControlType.ReadonlyCheckboxes:
                        {
                            //load read-only (already server-side selected) values
                            var attributeValues = addressAttributeService.GetAddressAttributeValues(attribute.Id);
                            foreach (var selectedAttributeId in attributeValues
                                .Where(v => v.IsPreSelected)
                                .Select(v => v.Id)
                                .ToList())
                            {
                                attributesXml = addressAttributeParser.AddAddressAttribute(attributesXml,
                                            attribute, selectedAttributeId.ToString());
                            }
                        }
                        break;
                    case AttributeControlType.TextBox:
                    case AttributeControlType.MultilineTextbox:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!String.IsNullOrEmpty(ctrlAttributes))
                            {
                                string enteredText = ctrlAttributes.Trim();
                                attributesXml = addressAttributeParser.AddAddressAttribute(attributesXml,
                                    attribute, enteredText);
                            }
                        }
                        break;
                    case AttributeControlType.Datepicker:
                    case AttributeControlType.ColorSquares:
                    case AttributeControlType.ImageSquares:
                    case AttributeControlType.FileUpload:
                    //not supported address attributes
                    default:
                        break;
                }
            }

            return attributesXml;
        }

        /// <summary>
        /// Lấy dạng URL của một chuỗi bất kỳ
        /// </summary>
        /// <param name="phrase">Chuỗi cần chuyển thành URl</param>
        /// <returns>Chuối dạng URL</returns>
        public static string ToURL(this string phrase)
        {
            string str = phrase.ToUnsignedVietnamese().RemoveAccent().ToLower().Replace("°", "").Replace(":", "");
            str = Regex.Replace(str, "&", " "); // invalid chars           
            str = Regex.Replace(str, @"[^a-z0-9\s-/?:]", ""); // invalid chars           
            str = Regex.Replace(str, @"\s+", " ").Trim(); // convert multiple spaces into one space   
            str = str.Substring(0, str.Length <= 150 ? str.Length : 150).Trim(); // cut and trim it   
            str = Regex.Replace(str, @"\s", "-"); // hyphens   

            return str;
        }

        /// <summary>
        /// Chuyển mãi về chuẩn ASCII để đảm bảo loại tất cả các dấu đặt biệt
        /// </summary>
        /// <param name="txt">Chuỗi cần chuyển</param>
        /// <returns>Chuỗi đã mã hóa</returns>
        public static string RemoveAccent(this string txt)
        {
            byte[] bytes = Encoding.GetEncoding("Cyrillic").GetBytes(txt);
            return Encoding.ASCII.GetString(bytes).Replace("-", " ").Replace("/", " ");
        }

        /// <summary>
        /// Bỏ dấu tiếng Việt của một chuỗi bất kỳ
        /// </summary>
        /// <param name="vietnamese">Chuỗi tiếng Việt cần bỏ dấu</param>
        /// <returns>Chuỗi đã khử dấu</returns>
        public static string ToUnsignedVietnamese(this string vietnamese)
        {
            if (vietnamese == null) return string.Empty;
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = vietnamese.Normalize(NormalizationForm.FormD);
            return regex.Replace(temp, string.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
        }
    }
}

