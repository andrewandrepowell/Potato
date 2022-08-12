using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Potato
{
    internal class SelectCombiner : ISelectable
    {
        public enum Options { Any, All };
        private IList<ISelectable> selectables;
        private Options option;
        public bool Selected
        {
            get
            {
                switch (option)
                {
                    case Options.Any: return selectables.Any((x) => x.Selected);
                    case Options.All: return selectables.All((x) => x.Selected);
                }
                throw new NotSupportedException();
            }
        }
        
        public SelectCombiner(IList<ISelectable> selectables, Options option)
        {
            this.selectables = selectables;
            this.option = option;
        }

        public void Select() => throw new NotImplementedException();
    }
}
