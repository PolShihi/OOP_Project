using MyModel.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientSideApp.Selectors
{
    class UserTemplateSelector : DataTemplateSelector
    {
        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var user = item as UserSession;
            var userTemplate = new object();


            if (user.Email == App.UserSession.Email)
            {
                Application.Current.Resources.TryGetValue("YouUserTemplate", out userTemplate);
            }
            else
            {
                Application.Current.Resources.TryGetValue("NotYouUserTemplate", out userTemplate);
            }

            return userTemplate as DataTemplate;
        }
    }
}
