using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Elementary_schools_overview.Models
{
    class School
    {
        private int id;
        private string name;
        private string adress;
        private string img_path;

        public string Name
        {
            get
            {
                return name;
            }
        }
        public int Id
        {
            get
            {
                return id;
            }
        }
        public string Img_path
        {
            get
            {
                return img_path;
            }
        }

        public Uri Img_Source
        {
            get
            {
                return new Uri("https://projekat.api.muharemovic.com/api/schools/" + img_path);
            }
        }

        public string Adress
        {
            get
            {
                return adress;
            }
        }

        public BitmapImage get_ImgIcon
        {
            get
            {
                return Functions.getImageSource("https://projekat.api.muharemovic.com/api/schools/" + img_path);
            }
        }
        public School(int id, string name, string adress, string img_path)
        {
            this.id = id;
            this.name = name;
            this.adress = adress;
            this.img_path = img_path;
        }

        public void EditSchool(string name, string adress)
        {
            this.name = name;
            this.adress = adress;
        }
    }
}

//------ DATABASE TABLE ----------
/*
CREATE TABLE projekat2_schools(
    id int(11) PRIMARY KEY NOT NULL AUTO_INCREMENT,
    name varchar(64) NOT NULL,
    adress varchar(64) NOT NULL,
    img_path varchar(64) NOT NULL
);
*/
//-------------------------------
