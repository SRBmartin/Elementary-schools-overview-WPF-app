using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Elementary_schools_overview.Models
{
    class Student
    {
        private string JMBG;
        private string name;
        private string surname;
        private string adress;
        private string img_path;
        private int school_id;

        public int School_id
        {
            get
            {
                return school_id;
            }
        }
        public string get_JMBG
        {
            get
            {
                return JMBG;
            }
        }
        public string Name
        {
            get
            {
                return name;
            }
        }
        public string Surname
        {
            get
            {
                return surname;
            }
        }
        public string Adress
        {
            get
            {
                return adress;
            }
        }
        public string Img_path
        {
            get
            {
                return img_path;
            }
            set
            {
                img_path = value;
            }
        }
        public BitmapImage get_ImgIcon
        {
            get
            {
                return Functions.getImageSource("https://projekat.api.muharemovic.com/api/students/getIcon/" + Img_path);
            }
        }


        public Student(string JMBG, string name, string surname, string adress, string img_path, int school_id)
        {
            this.JMBG = JMBG;
            this.name = name;
            this.surname = surname;
            this.adress = adress;
            this.img_path = img_path;
            this.school_id = school_id;
        }

        public void ChangeSchool(int school_id_for_change)
        {
            school_id = school_id_for_change;
        }

        public void updateStudent(string name, string surname, string adress)
        {
            this.name = name;
            this.surname = surname;
            this.adress = adress;
        }

        public void OnStudentDeleted(string JMBG)
        {

        }

    }
}

/* ---------- DATA TABLE -------------*/
/*
CREATE TABLE projekat2_students(
  	JMBG varchar(14) NOT NULL PRIMARY KEY,
    name varchar(32) NOT NULL,
    surname varchar(32) NOT NULL,
    adress varchar(64) NOT NULL,
    img_path varchar(64) NOT NULL,
    enrolled_in int(11) NOT NULL,
    FOREIGN KEY (enrolled_in) REFERENCES projekat2_schools(id)
);
ALTER TABLE projekat2_students ALTER COLUMN enrolled_in SET DEFAULT 1
*/
/*------------------------------------*/