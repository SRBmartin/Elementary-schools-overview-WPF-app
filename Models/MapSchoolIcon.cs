using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Elementary_schools_overview.Models
{
    class MapSchoolIcon
    {
        private Point position;
        private BitmapImage imgIcon;
        private int school_id;
        public Point Position
        {
            get
            {
                return position;
            }
        }

        public int School_id
        {
            get
            {
                return school_id;
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
        public MapSchoolIcon(Point position, string imgPath, int school_id)
        {
            this.position = position;
            this.school_id = school_id;
            imgIcon = Functions.getImageSource("https://projekat.api.muharemovic.com/api/schools/" + imgPath);
        }
        public MapSchoolIcon(int school_id, double x, double y, string img_path)
        {
            this.school_id = school_id;
            position = new Point(x, y);
            imgIcon = Functions.getImageSource("https://projekat.api.muharemovic.com/api/schools/" + img_path);
        }
    }
}

// --------------- DATA TABLE DEFINICIJA ----------------------//
/*CREATE TABLE projekat2_school_icons(
  	school_id int(11) PRIMARY KEY NOT NULL,
    X double NOT NULL,
    Y double NOT NULL,
    img_path varchar(128) NOT NULL
);*/
//------------------------------------------------------------//