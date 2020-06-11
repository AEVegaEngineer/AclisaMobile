﻿using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms.Internals;

namespace AclisaMobile.Services
{
    [Preserve(AllMembers = true)]
    public interface ILoginManager
    {
        void ShowMainPage();
        void Logout();
    }
}
