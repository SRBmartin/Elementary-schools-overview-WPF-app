using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elementary_schools_overview.Models
{
    class MapSchoolIcons
    {
        private List<MapSchoolIcon> school_icons;

        public List<MapSchoolIcon> School_icons
        {
            get
            {
                return school_icons;
            }
        }
        public MapSchoolIcons()
        {
            school_icons = new List<MapSchoolIcon>();
        }

        public bool isSchoolAlreadyOnMap(School draggedSchool)
        {
            if (draggedSchool != null)
            {
                for (int i = 0; i < school_icons.Count; ++i)
                {
                    if (draggedSchool.Id == school_icons[i].School_id)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void removeSchoolIconFromList(School forDelete)
        {
            for(int i = 0; i < school_icons.Count; ++i)
            {
                if(school_icons[i].School_id == forDelete.Id)
                {
                    school_icons.RemoveAt(i);
                }
            }
        }

        public void OnSchoolDeleted(School DeletedSchool)
        {
            if (isSchoolAlreadyOnMap(DeletedSchool))
            {
                removeSchoolIconFromList(DeletedSchool);
            }
        }

        public bool InsertMapIcon(License lic, MapSchoolIcon newIcon)
        {
            if (lic.InsertSchoolMapIcon(newIcon))
            {
                school_icons.Add(newIcon);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool RemoveSchoolFromMap(License lic, int schoolId)
        {
            if (lic.RemoveSchoolFromMap(schoolId))
            {
                for(int i = 0; i < school_icons.Count; ++i)
                {
                    if(school_icons[i].School_id == schoolId)
                    {
                        school_icons.RemoveAt(i);
                        break;
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
