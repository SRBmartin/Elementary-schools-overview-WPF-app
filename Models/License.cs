using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Elementary_schools_overview.Models
{
    class License
    {
        private string licenseKey;
        private string nextAction;

        private bool updateNextAction()
        {
            bool success = false;
            string result = ApiHandler.SendHttpRequest("verifyLicense/" + licenseKey, ApiHandler.Http_Method.POST, ref success);
            if (result.Length != 0)
            {
                var toCheck = Functions.JSON_converter(result);
                int status = Convert.ToInt32(toCheck["status"]);
                if (status == 200)
                {
                    nextAction = toCheck["next_action"];
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public string LicenseKey
        {
            get
            {
                return licenseKey;
            }
        }

        public string VerificationString
        {
            get
            {
                return licenseKey + "/" + nextAction;
            }
        }

        public License()
        {
            StreamReader sr = new StreamReader("./../../Files/license.txt");
            licenseKey = sr.ReadLine();
            sr.Close();
            if (licenseKey != "")
            {
                bool success = false;
                string result = ApiHandler.SendHttpRequest("verifyLicense/" + licenseKey, ApiHandler.Http_Method.POST, ref success);
                if (result.Length != 0)
                {
                    var toCheck = Functions.JSON_converter(result);
                    int status = Convert.ToInt32(toCheck["status"]);
                    if(status == 200)
                    {
                        nextAction = toCheck["next_action"];
                    }else if(status == 400)
                    {
                        throw new Exception("The license key is not in valid form.");
                    }
                    else if(status == 404)
                    {
                        throw new Exception("Could not find the license key in database.");
                    }
                }
                else
                {
                    throw new Exception("Could not find the license key in the database system.");
                }
            }
            else
            {
                throw new Exception("Could not find the license key in the file system.");
            }
        }


        public bool init_schools(Schools schools)
        {
            try
            {
                bool success = false;
                string result = ApiHandler.SendHttpRequest("schools/" + VerificationString, ApiHandler.Http_Method.GET, ref success);
                if (!updateNextAction())
                {
                    return false;
                }
                if (result.Length != 0)
                {
                    if (success)
                    {
                        do
                        {
                            Dictionary<string, string> tmp = Functions.JSON_converter(result);
                            School newSchool = new School(Convert.ToInt32(tmp["id"]), tmp["name"], tmp["adress"], tmp["img_path"]);
                            schools.addSchool(newSchool);
                        } while (Functions.checkConversionCondition(ref result));
                    }
                    else
                    {
                        return false;
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool init_students(Students students)
        {
            try
            {
                bool status = false;
                string result = ApiHandler.SendHttpRequest("students/" + VerificationString, ApiHandler.Http_Method.GET, ref status);
                if (!updateNextAction())
                {
                    return false;
                }
                if (result.Length != 0)
                {
                    do
                    {
                        Dictionary<string, string> tmp = Functions.JSON_converter(result);
                        Student newStudent = new Student(tmp.ElementAt(0).Value, tmp.ElementAt(1).Value, tmp.ElementAt(2).Value, tmp.ElementAt(3).Value, tmp.ElementAt(4).Value, Convert.ToInt32(tmp.ElementAt(5).Value));
                        students.addStudent(newStudent);
                    } while (Functions.checkConversionCondition(ref result));
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Init_school_map_icons(MapSchoolIcons map_icons)
        {
            try
            {
                bool status = false;
                string result = ApiHandler.SendHttpRequest("map/getSchoolIcons/" + VerificationString, ApiHandler.Http_Method.GET, ref status);
                if (!updateNextAction())
                {
                    return false;
                }
                if (result.Length != 0)
                {
                    do
                    {
                        Dictionary<string, string> tmp = Functions.JSON_converter(result);
                        MapSchoolIcon newSchoolIcon = new MapSchoolIcon(Convert.ToInt32(tmp.ElementAt(0).Value), Convert.ToDouble(tmp.ElementAt(1).Value), Convert.ToDouble(tmp.ElementAt(2).Value), tmp.ElementAt(3).Value);
                        map_icons.School_icons.Add(newSchoolIcon);
                    } while (Functions.checkConversionCondition(ref result));
                }//no need for else because this means it has no data available
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Init_student_map_icons(MapStudentIcons student_icons)
        {
            try
            {
                bool status = false;
                string result = ApiHandler.SendHttpRequest("map/getStudentIcons/" + VerificationString, ApiHandler.Http_Method.GET, ref status);
                if (!updateNextAction())
                {
                    return false;
                }
                if (result.Length != 0)
                {
                    do
                    {
                        Dictionary<string, string> tmp = Functions.JSON_converter(result);
                        MapStudentIcon newStudentIcon = new MapStudentIcon(tmp.ElementAt(0).Value, Convert.ToDouble(tmp.ElementAt(1).Value), Convert.ToDouble(tmp.ElementAt(2).Value), tmp.ElementAt(3).Value);
                        student_icons.Student_icons.Add(newStudentIcon);
                    } while (Functions.checkConversionCondition(ref result));
                }//no need for else because this means it has no data available
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool AddSchool(Schools schools, School targetSchool)
        {
            try
            {
                bool status = false;
                string result = ApiHandler.SendHttpRequest("schools/add/" + targetSchool.Name + "/" + targetSchool.Adress + "/" + VerificationString, ApiHandler.Http_Method.POST, ref status);
                if (!updateNextAction())
                {
                    return false;
                }
                if(result.Length != 0)
                {
                    if (!status)
                    {
                        return false;
                    }
                    Dictionary<string, string> tmp = Functions.JSON_converter(result);
                    schools.addSchool(new School(Convert.ToInt32(tmp["newSchoool_id"]), targetSchool.Name, targetSchool.Adress, tmp["img_path"]));
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public bool UpdateSchool(ref List<School> schools, int SchoolListIndex, string newName, string newAdress)
        {
            try
            {
                bool status = false;
                School targetSchool = schools[SchoolListIndex];
                string result = ApiHandler.SendHttpRequest("schools/edit/" + targetSchool.Id + "/" + newName + "/" + newAdress + "/" + VerificationString, ApiHandler.Http_Method.POST, ref status);
                if (!updateNextAction())
                {
                    return false;
                }
                if (!status)
                {
                    return false;
                }
                schools[SchoolListIndex].EditSchool(newName, newAdress);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteSchool(ref List<School> schools, int schoolListIndex)
        {
            try
            {
                bool status = false;
                int school_id = schools[schoolListIndex].Id;
                string result = ApiHandler.SendHttpRequest("schools/delete/" + school_id.ToString() + "/" + VerificationString,ApiHandler.Http_Method.POST, ref status);
                if (!updateNextAction())
                {
                    return false;
                }
                if (!status)
                {
                    return false;
                }
                schools.RemoveAt(schoolListIndex);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool AddNewStudent(ref List<Student> students, Student newStudent, string gender)
        {
            try
            {
                bool status = false;
                string apiUri = "students/add/" + newStudent.get_JMBG + "/" + newStudent.Name + "/" + newStudent.Surname + "/" + newStudent.Adress + "/" + gender + "/" + VerificationString;
                string result = ApiHandler.SendHttpRequest(apiUri, ApiHandler.Http_Method.POST, ref status);
                if (!updateNextAction())
                {
                    return false;
                }
                if (!status)
                {
                    return false;
                }
                Dictionary<string, string> tmp = Functions.JSON_converter(result);
                if (tmp["status"] == "id exists" || tmp["status"] == "failure")
                {
                    return false;
                }
                newStudent.Img_path = tmp["img_path"];
                students.Add(newStudent);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool EditStudent(ref List<Student> students, Student newStudentInfo)
        {
            try
            {
                bool status = false;
                string result = ApiHandler.SendHttpRequest("students/editStudent/" + newStudentInfo.get_JMBG + "/" + newStudentInfo.Name + "/" + newStudentInfo.Surname + "/" + newStudentInfo.Adress + "/" + VerificationString, ApiHandler.Http_Method.POST,ref status);
                if (!updateNextAction())
                {
                    return false;
                }
                if (!status)
                {
                    return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteStudent(string studentJMBG)
        {
            try
            {
                bool status = false;
                string result = ApiHandler.SendHttpRequest("students/deleteStudent/" + studentJMBG + "/" + VerificationString, ApiHandler.Http_Method.POST, ref status);
                if (!updateNextAction())
                {
                    return false;
                }
                if (!status)
                {
                    return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool SwitchSchool(Student targetStudent, int schoolId)
        {
            try
            {
                bool status = false;
                string result = ApiHandler.SendHttpRequest("students/changeSchool/" + targetStudent.get_JMBG + "/" + schoolId + "/" + VerificationString, ApiHandler.Http_Method.POST, ref status);
                if (!updateNextAction())
                {
                    return false;
                }
                if (!status)
                {
                    return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool InsertSchoolMapIcon(MapSchoolIcon newIcon)
        {
            try
            {
                bool status = false;
                string result = ApiHandler.SendHttpRequest("map/addSchool/" + newIcon.School_id + "/" + newIcon.PositionX + "/" + newIcon.PositionY + "/" + newIcon.ImgIcon.UriSource.ToString().Substring(newIcon.ImgIcon.UriSource.ToString().LastIndexOf('/') + 1) + "/" + VerificationString, ApiHandler.Http_Method.POST, ref status);
                if (!updateNextAction())
                {
                    return false;
                }
                if (!status)
                {
                    return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool InsertStudentIcon(MapStudentIcon newIcon)
        {
            try
            {
                bool status = false;
                string result = ApiHandler.SendHttpRequest("map/addStudent/" + newIcon.JMBG + "/" + newIcon.PositionX + "/" + newIcon.PositionY + "/" + newIcon.ImgIcon.UriSource.ToString().Substring(newIcon.ImgIcon.UriSource.ToString().LastIndexOf('/') + 1) + "/" + VerificationString, ApiHandler.Http_Method.POST, ref status);
                if (!updateNextAction())
                {
                    return false;
                }
                if (!status)
                {
                    return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool RemoveSchoolFromMap(int schoolId)
        {
            try
            {
                bool status = false;
                string result = ApiHandler.SendHttpRequest("map/removeSchool/" + schoolId + "/" + VerificationString, ApiHandler.Http_Method.POST, ref status);
                if (!updateNextAction())
                {
                    return false;
                }
                if (!status)
                {
                    return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool RemoveStudentFromMap(string studentJMBG)
        {
            try
            {
                bool status = false;
                string result = ApiHandler.SendHttpRequest("map/removeStudent/" + studentJMBG + "/" + VerificationString, ApiHandler.Http_Method.POST, ref status);
                if (!updateNextAction())
                {
                    return false;
                }
                if (!status)
                {
                    return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}


/*------- DATA TABLE ----------*/
/*
 CREATE TABLE projekat2_licenses(
  	license_id varchar(64) NOT NULL PRIMARY KEY,
    license_next_action varchar(64) NOT NULL
);
*/
/*----------------------------*/