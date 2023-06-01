using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elementary_schools_overview.Models
{
    class MapStudentIcons
    {
        List<MapStudentIcon> student_icons;

        public List<MapStudentIcon> Student_icons
        {
            get
            {
                return student_icons;
            }
        }
        public MapStudentIcons()
        {
            student_icons = new List<MapStudentIcon>();
        }
        public bool isStudentAlreadyOnMap(Student draggedStudent)
        {
            if (draggedStudent != null)
            {
                for (int i = 0; i < student_icons.Count; ++i)
                {
                    if (draggedStudent.get_JMBG == student_icons[i].JMBG)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void removeStudentIconFromList(Student forDelete)
        {
            for (int i = 0; i < student_icons.Count; ++i)
            {
                if (student_icons[i].JMBG == forDelete.get_JMBG)
                {
                    student_icons.RemoveAt(i);
                }
            }
        }

        public bool InsertMapIcon(License lic, MapStudentIcon newIcon)
        {
            if (lic.InsertStudentIcon(newIcon))
            {
                student_icons.Add(newIcon);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool RemoveStudentFromMap(License lic, string studentJMBG)
        {
            if (lic.RemoveStudentFromMap(studentJMBG))
            {
                for(int i = 0; i < student_icons.Count; ++i)
                {
                    if (student_icons[i].JMBG == studentJMBG)
                    {
                        student_icons.RemoveAt(i);
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
