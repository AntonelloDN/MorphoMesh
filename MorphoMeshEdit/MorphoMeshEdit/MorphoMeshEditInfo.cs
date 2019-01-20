using System;
using System.Drawing;
using Grasshopper.Kernel;

namespace MorphoMeshEdit
{
    public class MorphoMeshEditInfo : GH_AssemblyInfo
    {
        public override string Name
        {
            get
            {
                return "MorphoMeshEdit";
            }
        }
        public override Bitmap Icon
        {
            get
            {
                //Return a 24x24 pixel bitmap to represent this GHA library.
                return null;
            }
        }
        public override string Description
        {
            get
            {
                //Return a short string describing the purpose of this GHA library.
                return "";
            }
        }
        public override Guid Id
        {
            get
            {
                return new Guid("049b3f69-257f-4b4a-8a1b-4b3f94a9ec54");
            }
        }

        public override string AuthorName
        {
            get
            {
                //Return a string identifying you or your company.
                return "";
            }
        }
        public override string AuthorContact
        {
            get
            {
                //Return a string representing your preferred contact details.
                return "";
            }
        }
    }
}
