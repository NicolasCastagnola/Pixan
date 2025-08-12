/*using System;
using System.Linq;
using GE.BehaviourTreeSystem;
using RotaryHeart.Lib.AutoComplete;
using UnityEngine;
using UnityEditor;

namespace Parasite
{
    //[CustomPropertyDrawer(typeof(AutoCompleteAttribute))]
    //[CustomPropertyDrawer(typeof(AutoCompleteTextFieldAttribute))]
    //[CustomPropertyDrawer(typeof(AutoCompleteDropDownAttribute))]
    [CustomPropertyDrawer(typeof(HashedString))]
    public class HashedStringDrawer : PropertyDrawer
    {
        enum AttributeType
        {
            TextField,
            Dropdown
        }

        string[] m_entries;
        AttributeType m_attributeType;

        // Draw the property inside the given rect
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
	        if (!GetShouldDraw(property))
	        {
		        EditorGUI.BeginProperty(position, label, property);
		        EditorGUI.PropertyField(position, property, label, true);
		        EditorGUI.EndProperty();
		        return;
	        }
	        
            var keyProp = property.FindPropertyRelative("key");
            if (m_entries == null)
            {
	            if (!GameManager.disableLogs) Debug.Log($">{keyProp.stringValue}");
	            var window = EditorWindow.GetWindow<BehaviourTreeEditorWindow>();

	            if (window && window.ActiveTree && window.ActiveTree.blackboard.defaultValues != null)
	            {
		            var list = window.ActiveTree.blackboard.defaultValues.ConvertAll(x => x.key.key);
		            if (!string.IsNullOrEmpty(keyProp.stringValue))
			            list.Insert(0, keyProp.stringValue);
		            m_entries = list.ToArray();
	            }
	            else if (!string.IsNullOrEmpty(keyProp.stringValue))
		            m_entries = new[] { keyProp.stringValue };
	            else
		            m_entries = new string[1]
		            {
			            keyProp.stringValue
		            };

	            if (System.Attribute.GetCustomAttribute(fieldInfo, typeof(AutoCompleteTextFieldAttribute)) != null)
	            {
		            m_attributeType = AttributeType.TextField;
	            }
	            else if (System.Attribute.GetCustomAttribute(fieldInfo, typeof(AutoCompleteDropDownAttribute)) !=
	                     null)
	            {
		            m_attributeType = AttributeType.Dropdown;
	            }
            }
            else
	            m_entries[0] = keyProp.stringValue;

            switch (m_attributeType)
            {
                case AttributeType.TextField:
	                if (!GameManager.disableLogs) Debug.Log($"AutoCompleteTextField: {keyProp.stringValue}");
                    m_entries[0] = keyProp.stringValue = AutoCompleteTextField.EditorGUI.AutoCompleteTextField(
	                    position,
	                    label,
	                    keyProp.stringValue,
	                    GUI.skin.textField,
	                    m_entries, 
	                    "Type something here",
	                    true,
	                    true);
	                if (!GameManager.disableLogs) Debug.Log(keyProp.stringValue);
                    break;
                case AttributeType.Dropdown:
	                if (!GameManager.disableLogs) Debug.Log("AutoCompleteDropDown");
                    AutoCompleteDropDown.EditorGUI.AutoCompleteDropDown(position, label, keyProp.stringValue,
                        m_entries, s =>
                        {
                            keyProp.stringValue = s;
                            keyProp.serializedObject.ApplyModifiedProperties();
                        }, true);
                    break;
            }
        }
        
        private bool GetShouldDraw(SerializedProperty property)
        {
	        // Check if the ConditionalDrawerAttribute is present in attributes
	        UseCustomDrawerAttribute attribute = fieldInfo.GetCustomAttributes(typeof(UseCustomDrawerAttribute), true).FirstOrDefault() as UseCustomDrawerAttribute;
	        return attribute == null || attribute.ShouldDraw;
        }
    }
}*/