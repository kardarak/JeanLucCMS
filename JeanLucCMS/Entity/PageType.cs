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
    
    public partial class PageType
    {
        public PageType()
        {
            this.Pages = new HashSet<Page>();
            this.Childrens = new HashSet<PageType>();
        }
    
        public System.Guid PageTypeId { get; set; }
        public Nullable<System.Guid> ParentPageTypeId { get; set; }
        public string Name { get; set; }
        public bool IsFolder { get; set; }
        public string ControllerType { get; set; }
        public string ActionName { get; set; }
        public string ModelType { get; set; }
    
        public virtual ICollection<Page> Pages { get; set; }
        public virtual ICollection<PageType> Childrens { get; set; }
        public virtual PageType Parent { get; set; }
    }
}
