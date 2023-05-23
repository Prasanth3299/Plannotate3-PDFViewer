using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RevolutionGames
{
    public class DataClass : MonoBehaviour
    {
        #region properties

        private static DataClass instance = null;

        public List<ProjectDetails> projectDetails = new List<ProjectDetails>();
        public List<LayerDatas> layerDatas = new List<LayerDatas>();

        #endregion

        #region Built in Methods

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }

            DontDestroyOnLoad(this);
        }

        #endregion

        #region Instance
        public static DataClass Instance()
        {
            return instance;
        }
        #endregion


        #region Data Class

        public class ProjectDetails
        {
            public string folder_id = "";
            public string project_id = "";
            public string document_path = "";
            public string folder_name = "";
            public string parent_folder_id = "";
            public int folder_level;
            public int is_folder_flag;
            public List<Pages> pages = new List<Pages>();
        }

        public class Pages
        {
            public string page_id = "";
            public string document_id = "";
            public string page_number="";
            public string page_name = "";
            public string width="";
            public string height="";
            public string file_path = "";
            public string page_scale = "";
            public string base_icon_size = "";
            public string active_layer_id = "";
        }

        public class LayerDatas
        {
            public string layer_id = "";
            public string project_id = "";
            public string document_id = "";
            public string layer_name = "";
            public string layer_type = "";
            public LayerData layerData = new LayerData();
            public int is_default_flag;
            public int is_active_flag;
            public int is_locked_flag;
            public int is_visible_flag;
            public int is_removed;
            public int created_by_user_id;
            public string last_updated_date = "";
            public int version_number;

        }
        public class LayerData
        {
            public string layer_id = "";
            public string project_id = "";
            public string document_id = "";
            public string layer_name = "";
            public string layer_type = "";
            public List<AnnotationPages> associated_pages = new List<AnnotationPages>();
            public List<Annotations> annotations = new List<Annotations>();
            public bool is_default_flag;
            public int is_active_flag;
            public bool is_locked_flag;
            public bool is_visible_flag;
            public bool is_removed;
            public string created_date = "";
        }

        public class AnnotationPages
        {
            public bool is_lock;
            public string page_id = "";
            public string layer_id = "";
            public bool is_hidden;
            public string is_removed;
            public string document_id = "";
            public string created_date = "";
            public int version_number;
            public string last_updated_date = "";
            public int created_by_user_id;
        }
        public class Annotations
        {
            public float opacity;
            public string page_id = "";
            public string layer_id = "";
            public string fill_color = "";
            public string is_removed;
            public float line_width;
            public string project_id = "";
            public string document_id = "";
            public string created_date = "";
            public float element_size;
            public string stroke_color = "";
            public string annotation_id = "";
            //public float initial_width;
            //public string[] annotation_url = null;
            //public float initial_height;
            public string version_number = "";
            public string annotation_data = "";
            public string annotation_name = "";
            public string annotation_tags = "";
            //public List<AnnotationForms> annotation_forms;
            public string annotation_label = "";
            //public List<AnnotationLinks> annotation_links;
            public List<AnnotationMedia> annotation_media;
            public List<AnnotationStubs> annotation_stubs;
            public string initial_rotation = "0";
            public string last_updated_date = "";
            public string created_by_user_id = "";
            //public float initial_position_x;
            //public float initial_position_y;
            public int toolbar_element_id;
        }
        public class AnnotationForms
        {
            public string form_id = "";
            public List<FormData> form_data;
            public string form_name = "";
            public int is_hidden;
            public int is_removed;
            public string project_id = "";
            public string created_date = "";
            public string annotation_id = "";
            public string version_number = "";
            public string isLocalModified = "";
            public string is_default_flag = "";
            public string last_updated_date = "";
            public int created_by_user_id;
            public string form_element_count = "";
        }
        public class FormData
        {
            public int edit_mode;
            public int is_hidden;
            public int element_id;
            public int is_removed;
            public ElementData element_data = new ElementData();
            public string element_name = "";
            public string element_type = "";
            public string element_uuid = "";
            public int element_order;
            public int version_number;
        }
        public class ElementData
        {
            public string if_do = "";
            //public string options = "";
            public List<Option> options;
            public string if_state = "";
            public string if_value = "";
            public string label_text = "";
            public string label_align = "";
            public string if_condition = "";
            public string reference_id = "";
            public string default_value = "";
            public string maximum_value = "";
            public string minimum_value = "";
            public string use_conditions = "";
            public string placeholder_text = "";
            public int number_of_columns;
            public string element_name_alias = "";
            public string last_modified_date = "";
            public string use_calculated_values = "";

        }
        public class Option
        {
            public string icon = "";
            public string name = "";
            public bool default1 = true;
            public float opacity;
            public bool attributes;
            public string fill_color = "";
            public bool is_removed;
            public float line_width;
            public string element_uuid = "";
            public string stroke_color = "";
            public string calculated_value = "";
            public string number_of_columns = "";
        }
        public class AnnotationLinks
        {
            public string location = "";
            public string link_path = "";
            public string link_type = "";
            public string is_removed = "";
            public string document_id = "";
            public string created_date = "";
            public string annotation_id = "";
            public int version_number;
            public string last_updated_date = "";
            public string annotation_link_id = "";
            public int created_by_user_id;
        }
        public class AnnotationMedia
        {
            public string stub_id = "";
            public string media_url = "";
            public bool is_removed;
            public string media_name = "";
            public string media_type = "";
            public string from_medium = "";
            public string created_date = "";
            public string annotation_id = "";
            public string media_comment = "";
            public string version_number = "";
            public string last_updated_date = "";
            public string created_by_user_id = "";
            public string annotation_media_id = "";
        }
        public class AnnotationStubs
        {

        }

        #endregion


    }
}

