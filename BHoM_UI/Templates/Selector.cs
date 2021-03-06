/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2018, the respective contributors. All rights reserved.
 *
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 *                                           
 *                                                                              
 * The BHoM is free software: you can redistribute it and/or modify         
 * it under the terms of the GNU Lesser General Public License as published by  
 * the Free Software Foundation, either version 3.0 of the License, or          
 * (at your option) any later version.                                          
 *                                                                              
 * The BHoM is distributed in the hope that it will be useful,              
 * but WITHOUT ANY WARRANTY; without even the implied warranty of               
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
 * GNU Lesser General Public License for more details.                          
 *                                                                            
 * You should have received a copy of the GNU Lesser General Public License     
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */

using BH.Engine.Reflection;
using BH.oM.Reflection;
using BH.Engine.UI;
using BH.oM.Reflection.Attributes;
using BH.oM.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BH.oM.DataStructure;
using System.Windows.Forms;
using BH.Engine.Serialiser;

namespace BH.UI.Templates
{
    public class Selector<T> : ISelector
    {
        /*************************************/
        /**** Events                      ****/
        /*************************************/

        public event EventHandler<object> ItemSelected;


        /*************************************/
        /**** Constructors                ****/
        /*************************************/

        public Selector(IEnumerable<T> possibleItems, string key) 
        {
            m_Key = key;

            if (!m_ItemTreeStore.ContainsKey(key) || !m_ItemListStore.ContainsKey(key))
            {
                List<T> toKeep = possibleItems.Where(x => x.IIsToKeepInMenu()).ToList();
                Output<List<SearchItem>, Tree<T>> organisedMethod = Engine.UI.Compute.OrganiseItems(toKeep);
                m_ItemListStore[key] = organisedMethod.Item1;
                m_ItemTreeStore[key] = organisedMethod.Item2;
            }
        }

        /*************************************/
        /**** Public Methods              ****/
        /*************************************/

        public void AddToMenu(ToolStripDropDown menu)
        {
            if (m_SelectorMenu == null)
                SetSelectorMenu(new SelectorMenu_WinForm<T>(m_ItemListStore[m_Key], m_ItemTreeStore[m_Key]));

            m_SelectorMenu.FillMenu(menu);
        }

        /*************************************/

        public void AddToMenu(System.Windows.Controls.ContextMenu menu)
        {
            if (m_SelectorMenu == null)
                SetSelectorMenu(new SelectorMenu_Wpf<T>(m_ItemListStore[m_Key], m_ItemTreeStore[m_Key]));

            m_SelectorMenu.FillMenu(menu);
        }

        /*************************************/

        public void AddToMenu(object menu)
        {
            if (m_SelectorMenu != null)
                m_SelectorMenu.FillMenu(menu);
        }

        /*************************************/

        public void SetSelectorMenu<M>(SelectorMenu<T, M> selectorMenu)
        {
            selectorMenu.SetItems(m_ItemListStore[m_Key], m_ItemTreeStore[m_Key]);
            selectorMenu.ItemSelected += M_Menu_ItemSelected;

            m_SelectorMenu = selectorMenu;
        }


        /*************************************/
        /**** Private Methods             ****/
        /*************************************/

        private void M_Menu_ItemSelected(object sender, T e)
        {
            ItemSelected?.Invoke(this, e);
        }


        /*************************************/
        /**** Private Fields              ****/
        /*************************************/

        protected string m_Key = "";
        protected ISelectorMenu<T> m_SelectorMenu = null;

        protected static Dictionary<string, Tree<T>> m_ItemTreeStore = new Dictionary<string, Tree<T>>();
        protected static Dictionary<string, List<SearchItem>> m_ItemListStore = new Dictionary<string, List<SearchItem>>();


        /*************************************/
    }
}
