/* 
 * transcripa
 * http://code.google.com/p/transcripa/
 * 
 * Copyright 2011, Bryan McKelvey
 * Licensed under the MIT license
 * http://www.opensource.org/licenses/mit-license.php
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml;

namespace transcripa
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public LanguageManager languageManager;
        public Accents accents;
        private const string windowTitle = "transcripa";

        public MainWindow()
        {
            int comboBoxIndex = 0;
            int counter = 0;
            string currentLanguage = Properties.Settings.Default.CurrentLanguage;
            string currentInput = Properties.Settings.Default.CurrentInput;
            int currentRomanizationIndex = Properties.Settings.Default.CurrentRomanizationIndex;

            InitializeComponent();
            textBoxInput.Text = currentInput;
            textBoxInput.SelectionStart = currentInput.Length;
            hideErrorsCheckBox.IsChecked = Properties.Settings.Default.HideErrors;

            languageManager = new LanguageManager();
            accents = new Accents();

            // Write available languages to the combo box
            foreach (string language in languageManager.Languages)
            {
                comboBoxLanguage.Items.Add(language);
                if (language == currentLanguage)
                {
                    comboBoxIndex = counter;
                }
                counter++;
            }

            // Select the first item if it exists
            if (languageManager.Length != 0)
            {
                comboBoxLanguage.SelectedIndex = comboBoxIndex;
            }

            comboBoxRomanization.ItemsSource = languageManager.CurrentLanguage.RomanizationNames;
            Visibility visibility = (comboBoxRomanization.Items.Count == 0 ? Visibility.Hidden : Visibility.Visible);
            labelRomanization.Visibility = visibility;
            comboBoxRomanization.Visibility = visibility;

            if (currentRomanizationIndex != -1 && currentRomanizationIndex < comboBoxRomanization.Items.Count)
            {
                comboBoxRomanization.SelectedIndex = currentRomanizationIndex;
            }

            Transcribe();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.CurrentInput = textBoxInput.Text;
            Properties.Settings.Default.Save();
        }

        private void textBoxInput_KeyUp(object sender, KeyEventArgs e)
        {
            string text = textBoxInput.Text;
            int start = textBoxInput.SelectionStart;

            if (e.Key == Key.Escape)
            {
                Close();       
            }
            else if ((e.Key < Key.A || e.Key > Key.Z))
            {
                int textLength = text.Length;

                for (int i = accents.MaxLength; i > 1; i--)
                {
                    if (textLength >= i && start >= i)
                    {
                        string unparsed = text.Substring(start - i, i);
                        string parsed = accents.Apply(unparsed);
                        if (parsed != unparsed)
                        {
                            text = text.Remove(start - i, i).Insert(start - i, parsed);
                            textBoxInput.Text = text;
                            textBoxInput.SelectionStart = start - 1;
                            break;
                        }
                    }
                }
            }

            Transcribe();
        }

        private void Transcribe()
        {
            string romanization = languageManager.CurrentLanguage.Romanize(textBoxInput.Text, false);
            textBoxRomanized.Text = romanization;
            textBoxOutput.Text = languageManager.CurrentLanguage.Transcribe(romanization, true, true);
        }

        private void comboBoxLanguage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string currentLanguage = comboBoxLanguage.SelectedValue.ToString();
            int romanizationIndex;
            int itemCount;

            languageManager.Load(currentLanguage);

            Title = string.Format("{0} - {1}", windowTitle, currentLanguage);
            Properties.Settings.Default.CurrentLanguage = currentLanguage;

            // Set romanization index
            romanizationIndex = languageManager.CurrentLanguage.RomanizationIndex;
            Properties.Settings.Default.CurrentRomanizationIndex = romanizationIndex;
            comboBoxRomanization.ItemsSource = languageManager.CurrentLanguage.RomanizationNames;
            Visibility visibility = (comboBoxRomanization.Items.Count == 0 ? Visibility.Hidden : Visibility.Visible);
            labelRomanization.Visibility = visibility;
            comboBoxRomanization.Visibility = visibility;
            itemCount = comboBoxRomanization.Items.Count;
            if (romanizationIndex != -1 && romanizationIndex < itemCount)
            {
                comboBoxRomanization.SelectedIndex = romanizationIndex;
            }
            else if (itemCount != 0)
            {
                comboBoxRomanization.SelectedIndex = 0;
            }
            else
            {
                // Make sure we run the transcription process
                Transcribe();
            }
            textBoxInput.Focus();
        }

        private void comboBoxRomanization_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int romanizationIndex = comboBoxRomanization.SelectedIndex;
            languageManager.CurrentLanguage.RomanizationIndex = romanizationIndex;
            Properties.Settings.Default.CurrentRomanizationIndex = romanizationIndex;
            Transcribe();
        }

        private void hideErrorsCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            Properties.Settings.Default.HideErrors = (cb.IsChecked.HasValue ? (bool)cb.IsChecked : false);
        }
    }
}
