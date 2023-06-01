using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Elementary_schools_overview.Models
{
    class MapStudentIcon
    {
        private string student_jmbg;
        private Point position;
        private BitmapImage imgIcon;

        public Point Position
        {
            get
            {
                return position;
            }
        }

        public string JMBG
        {
            get
            {
                return student_jmbg;
            }
        }

        public double PositionX
        {
            get
            {
                return position.X;
            }
        }
        public double PositionY
        {
            get
            {
                return position.Y;
            }
        }
        public BitmapImage ImgIcon
        {
            get
            {
                return imgIcon;
            }
        }
        public MapStudentIcon(Point position, string imgPath, string jmbg)
        {
            this.position = position;
            this.student_jmbg = jmbg;
            imgIcon = Functions.getImageSource("https://projekat.api.muharemovic.com/api/students/getIcon/" + imgPath);
        }
        public MapStudentIcon(string jmbg, double x, double y, string img_path)
        {
            student_jmbg = jmbg;
            position = new Point(x, y);
            imgIcon = Functions.getImageSource("https://projekat.api.muharemovic.com/api/students/getIcon/" + img_path);
        }
    }
}
// ---------------- DATA TABLE DEFINICIJA ---------------//
/*CREATE TABLE projekat2_student_icons(
  	student_jmbg varchar(15) PRIMARY KEY NOT NULL,
    X double NOT NULL,
    Y double NOT NULL,
    img_path varchar(128) NOT NULL
);*/
//--------------------------------------------------------//