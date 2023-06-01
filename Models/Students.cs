using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Elementary_schools_overview.Models
{
    class Students
    {
        private List<Student> students;

        public List<Student> get_Students
        {
            get
            {
                return students;
            }
        }
        public Students()
        {
            students = new List<Student>();
        }
        public void addStudent(Student newStudent)
        {
            students.Add(newStudent);
        }

        public List<Student> group_students_by_school(School targetSchool)
        {
            List<Student> ret = new List<Student>();
            for(int i = 0; i < students.Count; ++i)
            {
                if(students.ElementAt(i).School_id == targetSchool.Id)
                {
                    ret.Add(students.ElementAt(i));
                }
            }
            return ret;
        }

        public Student getStudentFromJMBG(string JMBG)
        {
            for(int i = 0; i < students.Count; ++i)
            {
                if(students[i].get_JMBG == JMBG)
                {
                    return students[i];
                }
            }
            return null;
        }

        public int getStudentListIndexFromJMBG(string JMBG)
        {
            for(int i = 0; i < students.Count; ++i)
            {
                if(students[i].get_JMBG == JMBG)
                {
                    return i;
                }
            }
            return -1;
        }

        public bool updateStudentWithStudentListIndex(int studentListIndex, string name, string surname, string adress)
        {
            try
            {
                students[studentListIndex].updateStudent(name, surname, adress);
                return true;
            }catch(Exception)
            {
                return false;
            }
        }

        public void removeStudentFromListWithListIndex(int studentListIndex)
        {
            students.RemoveAt(studentListIndex);
        }

        public void OnSchoolDeleted(int school_id)
        {
            for (int i = 0; i < students.Count; ++i)
            {
                if (students[i].School_id == school_id)
                {
                    students[i].ChangeSchool(1);
                }
            }
        }

        public bool JMBGExists(string jmbg)
        {
            foreach(Student i in students)
            {
                if(i.get_JMBG == jmbg)
                {
                    return true;
                }
            }
            return false;
        }

        public bool AddNewStudent(License lic, Student newStudent, string gender)
        {
            return lic.AddNewStudent(ref students, newStudent, gender);
        }

        public bool EditStudent(License lic, Student newStudentInfo)
        {
            return lic.EditStudent(ref students, newStudentInfo);
        }

        public bool DeleteStudent(License lic, string studentJMBG)
        {
            return lic.DeleteStudent(studentJMBG);
        }

        public bool SwitchSchool(License lic, Student targetStudent, int SchoolId)
        {
            return lic.SwitchSchool(targetStudent, SchoolId);
        }

    }
}
