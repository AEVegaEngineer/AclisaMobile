using System;

using AclisaMobile.Models;
using Xamarin.Forms.Internals;

namespace AclisaMobile.ViewModels
{
    [Preserve(AllMembers = true)]
    public class ItemDetailViewModel : BaseViewModel
    {
        public Item Item { get; set; }
        public ItemDetailViewModel(Item item = null)
        {
            Title = item?.Text;
            Item = item;
        }
    }
}
