using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Elementary_schools_overview.Models
{
    class Schools
    {
        private List<School> schools;
        public Schools()
        {
            schools = new List<School>();
        }
        public List<School> get_Schools
        {
            get
            {
                return schools;
            }
        }
        public void addSchool(School newSchool)
        {
            schools.Add(newSchool);
        }
        public School getSchoolFromName(string name)
        {
            for(int i = 0; i < schools.Count; ++i)
            {
                if(name == schools[i].Name)
                {
                    return schools[i];
                }
            }
            return null;
        }

        public void repopulate_tab2_comboBox(ComboBox cb)
        {
            cb.Items.Clear();
            for(int i = 0; i < schools.Count; ++i)
            {
                cb.Items.Add(schools[i].Name);
            }
        }

        public void removeSchoolAtIndex(int index)
        {
            schools.RemoveAt(index);
        }

        public int GetSchoolListIndexFromId(int schoolId)
        {
            for(int i = 0; i < schools.Count; ++i)
            {
                if(schools[i].Id == schoolId)
                {
                    return i;
                }
            }
            return -1;
        }

        public School getSchoolFromSchoolId(int school_id)
        {
            for(int i = 0; i < schools.Count; ++i)
            {
                if(schools[i].Id == school_id)
                {
                    return schools[i];
                }
            }
            return null;
        }

        public void populateComboBoxWithSchools(ComboBox cb)
        {
            foreach (School i in schools)
            {
                cb.Items.Add(i.Name);
            }
        }

        public bool SaveNewSchool(License lic, School newSchool)
        {
            return lic.AddSchool(this, newSchool);
        }
        public bool UpdateSchool(License lic, int schoolListIndex, string newName, string newAdress)
        {
            return lic.UpdateSchool(ref schools, schoolListIndex, newName, newAdress);
        }

        public bool DeleteSchool(License lic, int schoolListIndex)
        {
            return lic.DeleteSchool(ref schools, schoolListIndex);
        }
    }
}
