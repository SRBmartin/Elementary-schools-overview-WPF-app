using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Elementary_schools_overview.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Win32;
using System.IO;

namespace Elementary_schools_overview
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private License license;
        private Schools schools;
        private Students students;
        private MapSchoolIcons school_icons;
        private MapStudentIcons student_icons;
        private string NotFoundImageSource = "/Files/Static/undefined.png";
        private object draggedItem;
        private object draggedItemRight;
        private object draggedSchoolItem;
        private object draggedStudentItem;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                license = new License();
                schools = new Schools();
                students = new Students();
                school_icons = new MapSchoolIcons();
                student_icons = new MapStudentIcons();
                if (!license.init_schools(schools))
                {
                    throw new Exception("There was an error while loading schools from database.");
                }
                if (!license.init_students(students))
                {
                    throw new Exception("There was an error while loading students from database.");
                }
                if (!license.Init_school_map_icons(school_icons))
                {
                    throw new Exception("There was an error while loading school icons from database.");
                }
                if (!license.Init_student_map_icons(student_icons))
                {
                    throw new Exception("There was an error while loading school icons from database.");
                }
                tab1_student_image.Source = Functions.getImageSource(NotFoundImageSource);
                schools.populateComboBoxWithSchools(tab1_school_combo_box);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Close();
            }
        }

        private void tab_control_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(e.Source is TabControl tbControl && tbControl.SelectedIndex != -1)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (e.RemovedItems.Count > 0)
                    {
                        TabItem newTab = tbControl.SelectedItem as TabItem;
                        TabItem oldTab = e.RemovedItems[0] as TabItem;
                        if(oldTab != newTab)
                        {
                            if (oldTab.Header.ToString() == "Overview")
                            {
                                if (tab1_btn_new_school.Content.ToString() == "Finish addition" && !tab1_btn_edit_school.IsEnabled && !tab1_btn_delete_school.IsEnabled)
                                {
                                    MessageBox.Show("Please finish the addition of the school first.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                }
                                else if (tab1_btn_student_add.Content.ToString() == "Finish addition" && !tab1_btn_student_edit.IsEnabled && !tab1_btn_student_delete.IsEnabled)
                                {
                                    MessageBox.Show("Please finish the addition of the student first.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                }
                                else
                                {
                                    tab1_school_combo_box.SelectedIndex = -1;
                                    tab1_data_grid.ItemsSource = null;
                                }
                            }
                            else if (oldTab.Header.ToString() == "Map")
                            {
                                tab3_lv_schools.ItemsSource = null;
                                tab3_lv_students.ItemsSource = null;
                                for (int i = 0; i < school_icons.School_icons.Count; ++i)
                                {
                                    tab3_ic_icons.Items.Remove(school_icons.School_icons[i]);
                                }
                                for (int i = 0; i < student_icons.Student_icons.Count; ++i)
                                {
                                    tab3_ic_icons.Items.Remove(student_icons.Student_icons[i]);
                                }
                            }//provere za oldTab

                            if(newTab.Header.ToString() == "Enroll/Switch")
                            {
                                tab2_lv_left_school.ItemsSource = null;
                                tab2_lv_right_school.ItemsSource = null;
                                tab2_lv_left_school.AllowDrop = false;
                                tab2_lv_right_school.AllowDrop = false;
                                schools.repopulate_tab2_comboBox(tab2_cb_left_school);
                                schools.repopulate_tab2_comboBox(tab2_cb_right_school);
                            }else if(newTab.Header.ToString() == "Map")
                            {
                                tab3_lv_schools.ItemsSource = schools.get_Schools;
                                for(int i = 0; i < school_icons.School_icons.Count; ++i)
                                {
                                    tab3_ic_icons.Items.Add(school_icons.School_icons[i]);
                                }
                                for(int i = 0; i < student_icons.Student_icons.Count; ++i)
                                {
                                    tab3_ic_icons.Items.Add(student_icons.Student_icons[i]);
                                }
                            }

                        }
                    }
                }));
            }
        }

        //-----------================-----------------===================--------------------//
        //===========----------------=================-------------------====================//
        //=-=-=-=-=-=-=-=-=-=-=-=-= EVENTS FOR FIRST TAB ('Overview') =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-//
        //===========----------------=================-------------------====================//
        //-----------================-----------------===================--------------------//
        private void tab1_school_combo_box_Selected(object sender, RoutedEventArgs e)
        {
            //----------- INITIAL CHECKS --------------------//
            int school_id = (sender as ComboBox).SelectedIndex;
            if (school_id == -1)
            {
                tab1_school_image.Source = Functions.getImageSource(NotFoundImageSource);
                tab1_tb_school_id.Text = "";
                tab1_tb_school_name.Text = "";
                tab1_tb_school_adress.Text = "";
                tab1_btn_delete_school.IsEnabled = true;
                tab1_btn_edit_school.IsEnabled = true;
                return;
            }
            if(schools.get_Schools[school_id].Id == 1)
            {
                tab1_btn_delete_school.IsEnabled = false;
                tab1_btn_edit_school.IsEnabled = false;
            }
            else
            {
                tab1_btn_delete_school.IsEnabled = true;
                tab1_btn_edit_school.IsEnabled = true;
            }
            //----------------------------------------------------//
            //------ Populating DataGrid with students ----------//
            List<Student> dg_students = students.group_students_by_school(schools.get_Schools.ElementAt(school_id));
            tab1_data_grid.ItemsSource = null; //reset if anything is present
            tab1_data_grid.ItemsSource = dg_students;
            //-----------------------------------------------------//
            //=-=-=-=-=-=-=-=- CHANGING SCHOOL'S ICON =-=-=-=-=-=-=-=//
            BitmapImage tmpImg = Functions.getImageSource(schools.get_Schools[school_id].Img_Source.ToString());
            tab1_school_image.Source = tmpImg;
            //=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-//
            //------- POPULATING TEXTBOXES FOR SELECTED SCHOOL ---------//
            tab1_tb_school_id.Text = schools.get_Schools.ElementAt(school_id).Id.ToString();
            tab1_tb_school_name.Text = schools.get_Schools.ElementAt(school_id).Name;
            tab1_tb_school_adress.Text = schools.get_Schools.ElementAt(school_id).Adress;
            //---------------------------------------------------------------------------//
        }

        private void tab1_btn_new_school_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //------------- LATER ADDED VALIDATION ---------------//
                if (!tab1_btn_student_delete.IsEnabled && !tab1_btn_student_edit.IsEnabled)
                {
                    MessageBox.Show("You are already adding a student, finish that first.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                //--------------------------------------------------------//
                if (tab1_btn_delete_school.IsEnabled && tab1_btn_edit_school.IsEnabled || tab1_school_combo_box.SelectedIndex == 0)
                {
                    MessageBoxResult check = MessageBox.Show("Do you want to unselect school and clear the fields for new input?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (check == MessageBoxResult.Yes)
                    {
                        tab1_data_grid.ItemsSource = null;
                        tab1_school_combo_box.SelectedItem = null;
                        tab1_tb_school_id.Text = "";
                        tab1_tb_school_name.Text = "";
                        tab1_tb_school_adress.Text = "";
                        tab1_btn_new_school.Content = "Finish addition";
                        tab1_btn_delete_school.IsEnabled = false;
                        tab1_btn_edit_school.IsEnabled = false;
                        tab1_school_combo_box.IsEnabled = false;
                        tab1_school_image.Source = Functions.getImageSource(NotFoundImageSource);
                    }
                    else
                    {
                        MessageBox.Show("You canceled new school input action.", "Cancelled", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {
                    MessageBoxResult check = MessageBox.Show("Do you want to add (Yes), continue (No) or cancel (Cancel) the addition of new school (all fields with reset on cancel)?", "Confirmation", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                    if (check == MessageBoxResult.Cancel)
                    {
                        tab1_tb_school_id.Text = "";
                        tab1_tb_school_name.Text = "";
                        tab1_tb_school_adress.Text = "";
                        tab1_btn_new_school.Content = "Add school";
                        tab1_btn_delete_school.IsEnabled = true;
                        tab1_btn_edit_school.IsEnabled = true;
                        tab1_school_combo_box.IsEnabled = true;
                    }
                    else if (check == MessageBoxResult.Yes)
                    {
                        if (tab1_tb_school_name.Text == "" || tab1_tb_school_adress.Text == "")
                        {
                            string text = "";
                            if (tab1_tb_school_name.Text == "" && tab1_tb_school_adress.Text == "")
                            {
                                text = "TextBoxes for name and for adress are empty.";
                            }
                            else if (tab1_tb_school_name.Text == "")
                            {
                                text = "TextBox for name is empty.";
                            }
                            else if (tab1_tb_school_adress.Text == "")
                            {
                                text = "TextBox for adress is empty.";
                            }
                            MessageBox.Show(text, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        else
                        {
                            if (tab1_tb_school_name.Text.Length < 3)
                            {
                                MessageBox.Show("School name is too short.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                            else if (tab1_tb_school_name.Text.Length > 64)
                            {
                                MessageBox.Show("School name is too long.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }else if(tab1_tb_school_adress.Text.Length < 3)
                            {
                                MessageBox.Show("School adress is too short.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                            else if(tab1_tb_school_adress.Text.Length > 64)
                            {
                                MessageBox.Show("School adress is too long.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                            else
                            {
                                string name = tab1_tb_school_name.Text,
                                    adress = tab1_tb_school_adress.Text;
                                School newSchool = new School(-1,name,adress,"none");
                                if (schools.SaveNewSchool(license, newSchool))
                                {
                                    newSchool = schools.get_Schools[schools.get_Schools.Count - 1];
                                    MessageBox.Show("You have added a school with success.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                                }
                                else
                                {
                                    MessageBox.Show("Something went wrong while adding a new school.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                    return;
                                }
                                tab1_tb_school_id.Text = newSchool.Id.ToString();
                                tab1_school_combo_box.Items.Add(name);
                                tab1_school_combo_box.SelectedIndex = schools.get_Schools.Count - 1;
                                tab1_btn_new_school.Content = "Add school";
                                tab1_btn_delete_school.IsEnabled = true;
                                tab1_btn_edit_school.IsEnabled = true;
                                tab1_school_combo_box.IsEnabled = true;
                                tab1_school_image.Source = newSchool.get_ImgIcon;
                                /*----------------------------------------------------*/
                            }
                        }
                    }
                }//if else nothing happens
            }
            catch
            {
                MessageBox.Show("There was an error while adding a school.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
            }
        }

        private void tab1_btn_edit_school_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                School selectedSchool = schools.getSchoolFromName((string)tab1_school_combo_box.SelectedItem);
                //-------------- INITIAL CHECKS --------------------//
                if (selectedSchool == null)
                {
                    MessageBox.Show("There is nothing selected for a change.", "Edit error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                //-------------------------------------------------------//
                if (tab1_tb_school_adress.Text == selectedSchool.Adress && tab1_tb_school_name.Text == selectedSchool.Name)
                {
                    MessageBox.Show("You didn't enter any changes.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else if (tab1_tb_school_adress.Text == "" && tab1_tb_school_name.Text == "")
                {
                    MessageBox.Show("You can't leave any field empty.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }else if(tab1_tb_school_name.Text.Length < 3)
                {
                    MessageBox.Show("School name is too short.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }else if(tab1_tb_school_adress.Text.Length < 3)
                {
                    MessageBox.Show("School adress is too short.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else if(tab1_tb_school_name.Text.Length > 64)
                {
                    MessageBox.Show("School name is too long.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else if(tab1_tb_school_adress.Text.Length > 64)
                {
                    MessageBox.Show("School adress is too long.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    string msgForMsgBox = "Are you sure you want to procced with these changes?\n";
                    msgForMsgBox += "Old name: " + selectedSchool.Name + "\nNew name: " + tab1_tb_school_name.Text + "\n";
                    msgForMsgBox += "Old adress" + selectedSchool.Adress + "\nNew adress: " + tab1_tb_school_adress.Text;
                    MessageBoxResult toCheck = MessageBox.Show(msgForMsgBox, "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (toCheck == MessageBoxResult.Yes)
                    {
                        int selectedIndex = tab1_school_combo_box.SelectedIndex;
                        if (schools.UpdateSchool(license, selectedIndex, tab1_tb_school_name.Text, tab1_tb_school_adress.Text))
                        {
                            //----------- RESETING THE COMBOBOX WITH UPDATED VALUES ---------------//
                            tab1_school_combo_box.Items.Clear();
                            schools.populateComboBoxWithSchools(tab1_school_combo_box);
                            tab1_school_combo_box.SelectedIndex = selectedIndex;
                            //-----------------------------------------------------------------------//
                            MessageBox.Show("The school " + schools.get_Schools[selectedIndex].Name + " was edited with success.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show("There was an error while editing a student.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    } //else is not needed
                }
            }
            catch
            {
                MessageBox.Show("There was an error while editing a school.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void tab1_btn_delete_school_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                School selectedSchool = schools.getSchoolFromName((string)tab1_school_combo_box.SelectedItem);
                //-------------- INICIJALNE PROVERE --------------------//
                if (selectedSchool == null)
                {
                    MessageBox.Show("There is nothing selected for a delete.", "Delete error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                //--------------------------------------------------------//
                int school_id = tab1_school_combo_box.SelectedIndex;
                string msgBoxString = "Are you sure you want to delete school?\nID: " + selectedSchool.Id + "\nName:" + selectedSchool.Name + "\nAdress: " + selectedSchool.Adress;
                MessageBoxResult toCheck = MessageBox.Show(msgBoxString, "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (toCheck == MessageBoxResult.Yes)
                {
                    if(schools.DeleteSchool(license, tab1_school_combo_box.SelectedIndex))
                    {
                        students.OnSchoolDeleted(selectedSchool.Id);
                        school_icons.OnSchoolDeleted(selectedSchool);
                        tab1_data_grid.ItemsSource = null;
                        tab1_tb_school_id.Text = "";
                        tab1_tb_school_name.Text = "";
                        tab1_tb_school_adress.Text = "";
                        tab1_school_combo_box.Items.RemoveAt(tab1_school_combo_box.SelectedIndex);
                        tab1_school_combo_box.SelectedIndex = -1;
                    }
                    else
                    {
                        MessageBox.Show("There was an error while deleting the school.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch
            {
                MessageBox.Show("There was an error while deleting the school.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void tab1_data_grid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dg = sender as DataGrid;
            if(dg.SelectedItem != null)
            {
                var selectedRow = (Student)dg.SelectedItem;
                string selectedJMBG = selectedRow.get_JMBG;
                Student selectedStudent = students.getStudentFromJMBG(selectedJMBG);
                tab1_tb_student_jmbg.Text = selectedStudent.get_JMBG;
                tab1_tb_student_name.Text = selectedStudent.Name;
                tab1_tb_student_surname.Text = selectedStudent.Surname;
                tab1_tb_student_adress.Text = selectedStudent.Adress;
                tab1_student_image.Source = selectedStudent.get_ImgIcon;
            }
            else
            {
                tab1_tb_student_jmbg.Text = "";
                tab1_tb_student_name.Text = "";
                tab1_tb_student_surname.Text = "";
                tab1_tb_student_adress.Text = "";
                tab1_student_image.Source = Functions.getImageSource(NotFoundImageSource);
            }
        }

        private void tab1_btn_student_add_Click(object sender, RoutedEventArgs e)
        {
            //------------- LATER ADDED VALIDATION ----------------------//
            if(!tab1_btn_delete_school.IsEnabled && !tab1_btn_edit_school.IsEnabled && tab1_school_combo_box.SelectedIndex != 0)
            {
                MessageBox.Show("You are already adding a new school, please finish that first.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            //---------------------------------------------------------------//
            if (tab1_btn_student_edit.IsEnabled && tab1_btn_student_delete.IsEnabled)
            {
                MessageBoxResult toCheck = MessageBox.Show("You will unselect the school and start adding student.\nAre you sure?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if(toCheck == MessageBoxResult.Yes)
                {
                    tab1_btn_student_add.Content = "Finish addition";
                    tab1_btn_student_delete.IsEnabled = false;
                    tab1_btn_student_edit.IsEnabled = false;
                    tab1_school_combo_box.IsEnabled = false;
                    tab1_data_grid.IsEnabled = false;
                    tab1_stack_panel_gender.IsEnabled = true;
                    tab1_tb_student_jmbg.IsEnabled = true;
                    tab1_school_combo_box.SelectedIndex = -1;
                    tab1_data_grid.ItemsSource = null;
                    tab1_student_image.Source = Functions.getImageSource(NotFoundImageSource);
                }
            }else if(!tab1_btn_student_edit.IsEnabled && !tab1_btn_student_delete.IsEnabled || tab1_btn_student_add.Content.ToString() != "Finish addition")
            {
                MessageBoxResult isFinish = MessageBox.Show("Do you want to add a student (Yes), cancel addition (cancel) or continue adding (No)", "Confirmation", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                if(isFinish == MessageBoxResult.Yes)
                {
                    if (tab1_tb_student_jmbg.Text == "" || tab1_tb_student_name.Text == "" || tab1_tb_student_surname.Text == "" || tab1_tb_student_adress.Text == "" || (tab1_rb_male.IsChecked == false && tab1_rb_female.IsChecked == false))
                    {
                        MessageBox.Show("You didn't fill out some field/s. Please fill them all", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    } else if (tab1_tb_student_jmbg.Text.Length > 13)
                    {
                        MessageBox.Show("Student JMBG is too long.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    } else if (tab1_tb_student_jmbg.Text.Length < 3)
                    {
                        MessageBox.Show("Student JMBG is too short.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else if (tab1_tb_student_name.Text.Length > 32)
                    {
                        MessageBox.Show("Student name is too long.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else if (tab1_tb_student_surname.Text.Length > 32)
                    {
                        MessageBox.Show("Student surname is too long.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else if (tab1_tb_student_name.Text.Length < 3)
                    {
                        MessageBox.Show("Student name is too short.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else if (tab1_tb_student_surname.Text.Length < 3)
                    {
                        MessageBox.Show("Student surname is too short.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else if (tab1_tb_student_adress.Text.Length > 64)
                    {
                        MessageBox.Show("Student adress is too long.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else if (tab1_tb_student_adress.Text.Length < 3)
                    {
                        MessageBox.Show("Student adress is too short.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }else if (students.JMBGExists(tab1_tb_student_jmbg.Text))
                    {
                        MessageBox.Show("Student with that JMBG is already in database."); // I have an backend check also in API but i don't handle that here,
                        //because of the way i defines my SendHttp function to works
                    }
                    else
                    {
                        Student newStudent = new Student(tab1_tb_student_jmbg.Text, tab1_tb_student_name.Text, tab1_tb_student_surname.Text, tab1_tb_student_adress.Text, "none", 1);
                        if (students.AddNewStudent(license, newStudent, (tab1_rb_male.IsChecked == true) ? "male" : "female"))
                        {
                            string gender = (tab1_rb_male.IsChecked == true) ? "He" : "She";
                            tab1_btn_student_add.Content = "Add student";
                            tab1_btn_student_delete.IsEnabled = true;
                            tab1_btn_student_edit.IsEnabled = true;
                            tab1_school_combo_box.IsEnabled = true;
                            tab1_data_grid.IsEnabled = true;
                            tab1_stack_panel_gender.IsEnabled = false;
                            tab1_tb_student_jmbg.IsEnabled = false;
                            tab1_tb_student_jmbg.Text = "";
                            tab1_tb_student_name.Text = "";
                            tab1_tb_student_surname.Text = "";
                            tab1_tb_student_adress.Text = "";
                            tab1_rb_male.IsChecked = false;
                            tab1_rb_female.IsChecked = false;
                            MessageBox.Show("You have successfully added a new student. " + gender +" is added to the UNREGISTERED list.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show("There was an error while writing a student to a database.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }else if(isFinish == MessageBoxResult.Cancel)
                {
                    tab1_btn_student_add.Content = "Add student";
                    tab1_btn_student_delete.IsEnabled = true;
                    tab1_btn_student_edit.IsEnabled = true;
                    tab1_school_combo_box.IsEnabled = true;
                    tab1_data_grid.IsEnabled = true;
                    tab1_stack_panel_gender.IsEnabled = false;
                    tab1_tb_student_jmbg.IsEnabled = false;
                    tab1_tb_student_jmbg.Text = "";
                    tab1_tb_student_name.Text = "";
                    tab1_tb_student_surname.Text = "";
                    tab1_tb_student_adress.Text = "";
                    tab1_rb_male.IsChecked = false;
                    tab1_rb_female.IsChecked = false;
                    MessageBox.Show("You have canceled adding action for a new student.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void tab1_btn_student_edit_Click(object sender, RoutedEventArgs e)
        {
            Student selectedStudent = students.getStudentFromJMBG(tab1_tb_student_jmbg.Text);
            if (tab1_tb_student_jmbg.Text != "" && selectedStudent != null)
            {
                if(tab1_tb_student_name.Text == selectedStudent.Name &&
                    tab1_tb_student_surname.Text == selectedStudent.Surname &&
                    tab1_tb_student_adress.Text == selectedStudent.Adress)
                {
                    MessageBox.Show("You didn't change anything for the selected student.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (tab1_tb_student_jmbg.Text == "" || tab1_tb_student_name.Text == "" || tab1_tb_student_surname.Text == "" || tab1_tb_student_adress.Text == "")
                {
                    MessageBox.Show("You didn't fill out some field/s. Please fill them all", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else if (tab1_tb_student_jmbg.Text.Length > 13)
                {
                    MessageBox.Show("Student JMBG is too long.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else if (tab1_tb_student_jmbg.Text.Length < 3)
                {
                    MessageBox.Show("Student JMBG is too short.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else if (tab1_tb_student_name.Text.Length > 32)
                {
                    MessageBox.Show("Student name is too long.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else if (tab1_tb_student_surname.Text.Length > 32)
                {
                    MessageBox.Show("Student surname is too long.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else if (tab1_tb_student_name.Text.Length < 3)
                {
                    MessageBox.Show("Student name is too short.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else if (tab1_tb_student_surname.Text.Length < 3)
                {
                    MessageBox.Show("Student surname is too short.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else if (tab1_tb_student_adress.Text.Length > 64)
                {
                    MessageBox.Show("Student adress is too long.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else if (tab1_tb_student_adress.Text.Length < 3)
                {
                    MessageBox.Show("Student adress is too short.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    string msgBoxText = "Are you sure you want to change the following?\nOld name:" + selectedStudent.Name + "\nNew name: " + tab1_tb_student_name.Text;
                    msgBoxText += "\nOld surname: " + selectedStudent.Surname + "\nNew surname: " + tab1_tb_student_surname.Text + "\nOld adress: " + selectedStudent.Adress;
                    msgBoxText += "\nNew adress: " + tab1_tb_student_adress.Text;
                    MessageBoxResult toCheck = MessageBox.Show(msgBoxText,"Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (toCheck == MessageBoxResult.Yes)
                    {
                        if (tab1_tb_student_name.Text == "" || tab1_tb_student_surname.Text == "" || tab1_tb_student_adress.Text == "" || tab1_tb_student_jmbg.Text == "" || (tab1_rb_female.IsChecked != false && tab1_rb_male.IsChecked != false))
                        {
                            MessageBox.Show("You left some empty fields, please check them.");
                        }
                        else
                        {
                            Student newStudentInfo = new Student(tab1_tb_student_jmbg.Text, tab1_tb_student_name.Text, tab1_tb_student_surname.Text, tab1_tb_student_adress.Text, selectedStudent.Img_path, selectedStudent.School_id);
                            if (students.EditStudent(license, newStudentInfo))
                            {
                                int students_index = students.getStudentListIndexFromJMBG(selectedStudent.get_JMBG);
                                if (students.updateStudentWithStudentListIndex(students_index, tab1_tb_student_name.Text, tab1_tb_student_surname.Text, tab1_tb_student_adress.Text))
                                {
                                    int lastSelectedRowIndex = tab1_data_grid.SelectedIndex;
                                    List<Student> dg_students = students.group_students_by_school(schools.get_Schools.ElementAt(tab1_school_combo_box.SelectedIndex));
                                    tab1_data_grid.ItemsSource = null; //reset if anything is present
                                    tab1_data_grid.ItemsSource = dg_students;
                                    tab1_data_grid.SelectedIndex = lastSelectedRowIndex;
                                    tab1_tb_student_jmbg.Text = students.get_Students[students_index].get_JMBG;
                                    tab1_tb_student_name.Text = students.get_Students[students_index].Name;
                                    tab1_tb_student_surname.Text = students.get_Students[students_index].Surname;
                                    tab1_tb_student_adress.Text = students.get_Students[students_index].Adress;
                                    tab1_student_image.Source = newStudentInfo.get_ImgIcon;
                                    MessageBox.Show("The student was changed successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                                }
                                else
                                {
                                    MessageBox.Show("There was an error while editing a student.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                }
                            }
                            else
                            {
                                MessageBox.Show("There was an error while editing a student.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }

                        }
                    }
                } //else ne treba jer se nece nista desiti ako se odustane od izmene
            }
            else
            {
                MessageBox.Show("There is no selected student.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void tab1_btn_student_delete_Click(object sender, RoutedEventArgs e)
        {
            Student selectedStudent = students.getStudentFromJMBG(tab1_tb_student_jmbg.Text);
            if (selectedStudent != null)
            {
                string msgBoxText = "Are you sure you want to delete the following student?\nJMBG: " + selectedStudent.get_JMBG + "\nName: ";
                msgBoxText += selectedStudent.Name + "\nSurname: " + selectedStudent.Surname + "\nAdress: " + selectedStudent.Adress;
                MessageBoxResult toCheck = MessageBox.Show(msgBoxText, "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (toCheck == MessageBoxResult.Yes)
                {
                    if (students.DeleteStudent(license, selectedStudent.get_JMBG))
                    {
                        int deletion_index = students.getStudentListIndexFromJMBG(selectedStudent.get_JMBG);
                        if (student_icons.isStudentAlreadyOnMap(students.get_Students[deletion_index]))
                        {
                            student_icons.removeStudentIconFromList(students.get_Students[deletion_index]);
                        }
                        students.removeStudentFromListWithListIndex(deletion_index);
                        List<Student> dg_students = students.group_students_by_school(schools.get_Schools.ElementAt(tab1_school_combo_box.SelectedIndex));
                        tab1_data_grid.ItemsSource = null; //reset if anything is present
                        tab1_data_grid.ItemsSource = dg_students;
                        MessageBox.Show("Student was successfully deleted from database.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("There was an error while deleting a student.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("There is no selected student.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        //-----------================-----------------===================--------------------//
        //===========----------------=================-------------------====================//
        //=-=-=-=-=-=-=-=-=-=-=-=-=-=- KRAJ EVENTA ZA PRVI TAB =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-//
        //===========----------------=================-------------------====================//
        //-----------================-----------------===================--------------------//

        private void tab2_cb_left_school_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int school_list_id = tab2_cb_left_school.SelectedIndex;
            if(school_list_id != -1)
            {
                if(school_list_id != tab2_cb_right_school.SelectedIndex)
                {
                    tab2_lv_left_school.AllowDrop = true;
                    List<Student> tv_students = students.group_students_by_school(schools.get_Schools[school_list_id]);
                    tab2_lv_left_school.ItemsSource = null;
                    tab2_lv_left_school.ItemsSource = tv_students;
                }
                else
                {
                    tab2_lv_left_school.AllowDrop = false;
                    tab2_cb_left_school.SelectedItem = null;
                    MessageBox.Show("You can't select same schools on the both side.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                tab2_lv_left_school.ItemsSource = null;
            }
        }

        private void tab2_cb_right_school_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int school_list_id = tab2_cb_right_school.SelectedIndex;
            if (school_list_id != -1)
            {
                if (school_list_id != tab2_cb_left_school.SelectedIndex)
                {
                    tab2_lv_right_school.AllowDrop = true;
                    List<Student> tv_students = students.group_students_by_school(schools.get_Schools[school_list_id]);
                    tab2_lv_right_school.ItemsSource = null;
                    tab2_lv_right_school.ItemsSource = tv_students;
                }
                else
                {
                    tab2_lv_right_school.AllowDrop = false;
                    tab2_cb_right_school.SelectedItem = null;
                    MessageBox.Show("You can't select same schools on the both side.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                tab2_lv_right_school.ItemsSource = null;
            }
        }

        public void ColumnHeaderFixSizeEvent(object sender, MouseEventArgs e)
        {
            e.Handled = true;
        }

        private void tab2_lv_left_school_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            draggedItem = tab2_lv_left_school.SelectedItem;
        }

        private void tab2_lv_left_school_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed && draggedItem != null)
            {
                DragDrop.DoDragDrop(tab2_lv_left_school, draggedItem, DragDropEffects.Move);
            }
        }

        private void tab2_lv_right_school_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(Student)) && draggedItemRight == null)
            {
                Student droppedStudent = e.Data.GetData(typeof (Student)) as Student;
                string droppedStudentJMBG = (e.Data.GetData(typeof(Student)) as Student).get_JMBG;
                int newSchoolId = schools.get_Schools[tab2_cb_right_school.SelectedIndex].Id;
                string msgBoxText = "Are you sure you want to transfer following student to " + schools.get_Schools[tab2_cb_right_school.SelectedIndex].Name + "?\n";
                msgBoxText += "JMBG: " + (e.Data.GetData(typeof(Student)) as Student).get_JMBG + "\nName: " + (e.Data.GetData(typeof(Student)) as Student).Name + "\nSurname: ";
                msgBoxText += (e.Data.GetData(typeof(Student)) as Student).Surname + "\nAdress: " + (e.Data.GetData(typeof(Student)) as Student).Adress;
                MessageBoxResult toCheck = MessageBox.Show(msgBoxText, "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if(toCheck == MessageBoxResult.Yes)
                {
                    if(students.SwitchSchool(license, droppedStudent, newSchoolId))
                    {
                        students.get_Students[students.getStudentListIndexFromJMBG(droppedStudentJMBG)].ChangeSchool(newSchoolId);
                        List<Student> tv_students = students.group_students_by_school(schools.get_Schools[tab2_cb_right_school.SelectedIndex]);
                        tab2_lv_right_school.ItemsSource = null;
                        tab2_lv_right_school.ItemsSource = tv_students;
                        tv_students = students.group_students_by_school(schools.get_Schools[tab2_cb_left_school.SelectedIndex]);
                        tab2_lv_left_school.ItemsSource = null;
                        tab2_lv_left_school.ItemsSource = tv_students;
                        MessageBox.Show("Student was successfully transfered.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("There was an error while transfering schools.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            draggedItem = null;
        }

        private void tab2_lv_left_school_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(Student)) && draggedItem == null)
            {
                Student droppedStudent = e.Data.GetData(typeof(Student)) as Student;
                string droppedStudentJMBG = (e.Data.GetData(typeof(Student)) as Student).get_JMBG;
                int newSchoolId = schools.get_Schools[tab2_cb_left_school.SelectedIndex].Id;
                string msgBoxText = "Are you sure you want to transfer following student to " + schools.get_Schools[tab2_cb_left_school.SelectedIndex].Name + "?\n";
                msgBoxText += "JMBG: " + (e.Data.GetData(typeof(Student)) as Student).get_JMBG + "\nName: " + (e.Data.GetData(typeof(Student)) as Student).Name + "\nSurname: ";
                msgBoxText += (e.Data.GetData(typeof(Student)) as Student).Surname + "\nAdress: " + (e.Data.GetData(typeof(Student)) as Student).Adress;
                MessageBoxResult toCheck = MessageBox.Show(msgBoxText, "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (toCheck == MessageBoxResult.Yes)
                {
                    if (students.SwitchSchool(license, droppedStudent, newSchoolId))
                    {
                        students.get_Students[students.getStudentListIndexFromJMBG(droppedStudentJMBG)].ChangeSchool(newSchoolId);
                        List<Student> tv_students = students.group_students_by_school(schools.get_Schools[tab2_cb_right_school.SelectedIndex]);
                        tab2_lv_right_school.ItemsSource = null;
                        tab2_lv_right_school.ItemsSource = tv_students;
                        tv_students = students.group_students_by_school(schools.get_Schools[tab2_cb_left_school.SelectedIndex]);
                        tab2_lv_left_school.ItemsSource = null;
                        tab2_lv_left_school.ItemsSource = tv_students;
                        MessageBox.Show("Student was successfully transfered.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("There was an error while transfering schools.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                draggedItemRight = null;
            }
        }

        private void tab2_lv_right_school_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            draggedItemRight = tab2_lv_right_school.SelectedItem;
        }

        private void tab2_lv_right_school_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && draggedItemRight != null)
            {
                DragDrop.DoDragDrop(tab2_lv_right_school, draggedItemRight, DragDropEffects.Move);
            }
        }

        private void tab3_lv_schools_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(tab3_lv_schools.SelectedItem != null)
            {
                School selectedSchool = tab3_lv_schools.SelectedItem as School;
                List<Student> lv_students = students.group_students_by_school(selectedSchool);
                tab3_lv_students.ItemsSource = lv_students;
            }
        }



        private void Image_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(School)) && draggedSchoolItem != null)
            {
                var droppedData = e.Data.GetData(typeof(School)) as School;
                MessageBoxResult toCheck = MessageBox.Show("Are you sure you want to add " + droppedData.Name + " to map?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (toCheck == MessageBoxResult.Yes)
                {
                    Point dropPosition = e.GetPosition((IInputElement)sender);
                    dropPosition.X += 50;
                    dropPosition.Y -= 10;
                    MapSchoolIcon newIcon = new MapSchoolIcon(dropPosition, droppedData.Img_path, droppedData.Id);
                    if (school_icons.InsertMapIcon(license, newIcon))
                    {
                        tab3_ic_icons.Items.Add(newIcon);
                        MessageBox.Show("Icon was added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("There was an error while adding a new map icon.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else if (e.Data.GetDataPresent(typeof(Student)) && draggedStudentItem != null)
            {
                var droppedData = e.Data.GetData(typeof(Student)) as Student;
                MessageBoxResult toCheck = MessageBox.Show("Are you sure you want to add " + droppedData.Name + " " + droppedData.Surname + " to map?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (toCheck == MessageBoxResult.Yes)
                {
                    Point dropPosition = e.GetPosition((IInputElement)sender);
                    dropPosition.X += 50;
                    dropPosition.Y -= 10;
                    MapStudentIcon newIcon = new MapStudentIcon(dropPosition, droppedData.Img_path, droppedData.get_JMBG);
                    if (student_icons.InsertMapIcon(license, newIcon))
                    {
                        tab3_ic_icons.Items.Add(newIcon);
                        MessageBox.Show("Icon was added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("There was an error while adding a new icon.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            draggedSchoolItem = null;
            draggedStudentItem = null;
        }

        private void tab3_lv_schools_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            draggedSchoolItem = tab3_lv_schools.SelectedItem;
            if (school_icons.isSchoolAlreadyOnMap(draggedSchoolItem as School))
            {
                draggedSchoolItem = null;
            }
        }


        private void tab3_lv_schools_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && draggedSchoolItem != null)
            {
                DragDrop.DoDragDrop(tab3_lv_schools, draggedSchoolItem, DragDropEffects.Move);
            }
        }

        private void tab3_lv_students_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            draggedStudentItem = tab3_lv_students.SelectedItem;
            if (student_icons.isStudentAlreadyOnMap(draggedStudentItem as Student))
            {
                draggedStudentItem = null;
            }
        }

        private void tab3_lv_students_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && draggedStudentItem != null)
            {
                DragDrop.DoDragDrop(tab3_lv_students, draggedStudentItem, DragDropEffects.Move);
            }
        }

        private void tab3_img_cm_removeFromMap_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;
            ContextMenu contextMenu = (ContextMenu)menuItem.Parent;
            Image clickedIcon = (Image)contextMenu.PlacementTarget;
            MapSchoolIcon selectedSchoolIcon = clickedIcon.DataContext as MapSchoolIcon;
            if (selectedSchoolIcon != null)
            {
                string msgBoxText = "Are you sure you want to remove from map the following school?\nId: " + selectedSchoolIcon.School_id + "\nName: ";
                msgBoxText += schools.getSchoolFromSchoolId(selectedSchoolIcon.School_id).Name + "\nAdress: " + schools.getSchoolFromSchoolId(selectedSchoolIcon.School_id).Adress;
                MessageBoxResult toCheck = MessageBox.Show(msgBoxText, "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (toCheck == MessageBoxResult.Yes)
                {
                    if (school_icons.RemoveSchoolFromMap(license, selectedSchoolIcon.School_id))
                    {
                        tab3_ic_icons.Items.Remove(selectedSchoolIcon);
                        MessageBox.Show("Icon was removed successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("There was an error while deleting school icon from map.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MapStudentIcon selectedStudentIcon = clickedIcon.DataContext as MapStudentIcon;
                if (selectedStudentIcon != null)
                {
                    string msgBoxText = "Are you sure you want to remove from map the following student?\nId: " + selectedStudentIcon.JMBG + "\nName: ";
                    msgBoxText += students.getStudentFromJMBG(selectedStudentIcon.JMBG).Name + "\nSurname: " + students.getStudentFromJMBG(selectedStudentIcon.JMBG).Surname;
                    msgBoxText += "\nAdress: " + students.getStudentFromJMBG(selectedStudentIcon.JMBG).Adress;
                    MessageBoxResult toCheck = MessageBox.Show(msgBoxText, "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (toCheck == MessageBoxResult.Yes)
                    {
                        if(student_icons.RemoveStudentFromMap(license, selectedStudentIcon.JMBG))
                        {
                            tab3_ic_icons.Items.Remove(selectedStudentIcon);
                            MessageBox.Show("Icon was removed successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show("There was an error while removing a student icon from the map.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
        }

        private void tab3_img_cm_deleteFromDB_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;
            ContextMenu contextMenu = (ContextMenu)menuItem.Parent;
            Image clickedIcon = (Image)contextMenu.PlacementTarget;
            MapSchoolIcon selectedSchoolIcon = clickedIcon.DataContext as MapSchoolIcon;
            if (selectedSchoolIcon != null)
            {
                if(selectedSchoolIcon.School_id == 1)
                {
                    MessageBox.Show("You can't delete that school.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                string msgBoxText = "Are you sure you want to delete from database the following school?\nId: " + selectedSchoolIcon.School_id + "\nName: ";
                msgBoxText += schools.getSchoolFromSchoolId(selectedSchoolIcon.School_id).Name + "\nAdress: " + schools.getSchoolFromSchoolId(selectedSchoolIcon.School_id).Adress;
                MessageBoxResult toCheck = MessageBox.Show(msgBoxText, "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (toCheck == MessageBoxResult.Yes)
                {
                    int schoolListIndex = schools.GetSchoolListIndexFromId(selectedSchoolIcon.School_id);
                    if (schoolListIndex == -1)
                    {
                        MessageBox.Show("It seems that that school doesn't exists.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    School targetSchool = schools.get_Schools[schoolListIndex];
                    if (schools.DeleteSchool(license, schoolListIndex))
                    {
                        students.OnSchoolDeleted(targetSchool.Id);
                        tab3_ic_icons.Items.Remove(selectedSchoolIcon);
                        school_icons.OnSchoolDeleted(targetSchool);
                        tab3_lv_schools.ItemsSource = null;
                        tab3_lv_students.ItemsSource = null;
                        tab3_lv_schools.ItemsSource = schools.get_Schools;
                        tab1_school_combo_box.Items.Clear();
                        schools.populateComboBoxWithSchools(tab1_school_combo_box);
                        MessageBox.Show("The selected school was deleted successfully.", "Success delete", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                    }
                    else
                    {
                        MessageBox.Show("There was an error while deleting a school from database.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MapStudentIcon selectedStudentIcon = clickedIcon.DataContext as MapStudentIcon;
                if (selectedStudentIcon != null)
                {
                    string msgBoxText = "Are you sure you want to delete from database the following student?\nJMBG: " + selectedStudentIcon.JMBG + "\nName: ";
                    msgBoxText += students.getStudentFromJMBG(selectedStudentIcon.JMBG).Name + "\nSurname: " + students.getStudentFromJMBG(selectedStudentIcon.JMBG).Surname;
                    msgBoxText += "\nAdress: " + students.getStudentFromJMBG(selectedStudentIcon.JMBG).Adress;
                    MessageBoxResult toCheck = MessageBox.Show(msgBoxText, "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (toCheck == MessageBoxResult.Yes)
                    {
                        if(students.DeleteStudent(license, selectedStudentIcon.JMBG))
                        {
                            students.removeStudentFromListWithListIndex(students.getStudentListIndexFromJMBG(selectedStudentIcon.JMBG));
                            tab3_ic_icons.Items.Remove(selectedStudentIcon);
                            student_icons.Student_icons.Remove(selectedStudentIcon);
                            tab3_lv_students.ItemsSource = null;
                            tab3_lv_students.ItemsSource = students.group_students_by_school(tab3_lv_schools.SelectedItem as School);
                            MessageBox.Show("The student was removed from database with success.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show("There was an error while deleting student from database.", "Error", MessageBoxButton.OK, MessageBoxImage.Error); ;
                        }
                    }
                }
            }
        }

    }
}
