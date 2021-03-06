//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace JeanLucCMS.Entity
{
    using System;
    using System.Collections.Generic;
    
    public partial class Page
    {
        public Page()
        {
            this.Childrens = new HashSet<Page>();
            this.PageLanguages = new HashSet<PageLanguage>();
        }
    
        public System.Guid PageId { get; set; }
        public Nullable<System.Guid> ParentPageId { get; set; }
        public Nullable<System.Guid> PageTypeId { get; set; }
    
        public virtual ICollection<Page> Childrens { get; set; }
        public virtual Page Parent { get; set; }
        public virtual PageType PageType { get; set; }
        public virtual ICollection<PageLanguage> PageLanguages { get; set; }
    }
}
