﻿#pragma checksum "..\..\HitSounds.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "95D6481001A305FCE3EDA52EE480F3F7C95615B7035F8E2A8F38263613D888F3"
//------------------------------------------------------------------------------
// <auto-generated>
//     Dieser Code wurde von einem Tool generiert.
//     Laufzeitversion:4.0.30319.42000
//
//     Änderungen an dieser Datei können falsches Verhalten verursachen und gehen verloren, wenn
//     der Code erneut generiert wird.
// </auto-generated>
//------------------------------------------------------------------------------

using BMBF_Manager;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace BMBF_Manager {
    
    
    /// <summary>
    /// HitSounds
    /// </summary>
    public partial class HitSounds : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 25 "..\..\HitSounds.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox Quest;
        
        #line default
        #line hidden
        
        
        #line 26 "..\..\HitSounds.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtbox;
        
        #line default
        #line hidden
        
        
        #line 30 "..\..\HitSounds.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox Sound;
        
        #line default
        #line hidden
        
        
        #line 31 "..\..\HitSounds.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox GoodHitSound;
        
        #line default
        #line hidden
        
        
        #line 32 "..\..\HitSounds.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox BadHitSounds;
        
        #line default
        #line hidden
        
        
        #line 33 "..\..\HitSounds.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox MenuMusic;
        
        #line default
        #line hidden
        
        
        #line 34 "..\..\HitSounds.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox MenuClickSound;
        
        #line default
        #line hidden
        
        
        #line 35 "..\..\HitSounds.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox FireWorks;
        
        #line default
        #line hidden
        
        
        #line 36 "..\..\HitSounds.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox LevelCleared;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/BMBF Manager;component/hitsounds.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\HitSounds.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 10 "..\..\HitSounds.xaml"
            ((BMBF_Manager.HitSounds)(target)).MouseMove += new System.Windows.Input.MouseEventHandler(this.Drag);
            
            #line default
            #line hidden
            return;
            case 2:
            
            #line 14 "..\..\HitSounds.xaml"
            ((System.Windows.Controls.Button)(target)).MouseEnter += new System.Windows.Input.MouseEventHandler(this.noDrag);
            
            #line default
            #line hidden
            
            #line 14 "..\..\HitSounds.xaml"
            ((System.Windows.Controls.Button)(target)).MouseLeave += new System.Windows.Input.MouseEventHandler(this.doDrag);
            
            #line default
            #line hidden
            
            #line 14 "..\..\HitSounds.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Close);
            
            #line default
            #line hidden
            return;
            case 3:
            
            #line 15 "..\..\HitSounds.xaml"
            ((System.Windows.Controls.Button)(target)).MouseEnter += new System.Windows.Input.MouseEventHandler(this.noDrag);
            
            #line default
            #line hidden
            
            #line 15 "..\..\HitSounds.xaml"
            ((System.Windows.Controls.Button)(target)).MouseLeave += new System.Windows.Input.MouseEventHandler(this.doDrag);
            
            #line default
            #line hidden
            
            #line 15 "..\..\HitSounds.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Mini);
            
            #line default
            #line hidden
            return;
            case 4:
            this.Quest = ((System.Windows.Controls.TextBox)(target));
            
            #line 25 "..\..\HitSounds.xaml"
            this.Quest.LostFocus += new System.Windows.RoutedEventHandler(this.QuestIPCheck);
            
            #line default
            #line hidden
            
            #line 25 "..\..\HitSounds.xaml"
            this.Quest.GotFocus += new System.Windows.RoutedEventHandler(this.ClearText);
            
            #line default
            #line hidden
            
            #line 25 "..\..\HitSounds.xaml"
            this.Quest.MouseEnter += new System.Windows.Input.MouseEventHandler(this.noDrag);
            
            #line default
            #line hidden
            
            #line 25 "..\..\HitSounds.xaml"
            this.Quest.MouseLeave += new System.Windows.Input.MouseEventHandler(this.doDrag);
            
            #line default
            #line hidden
            return;
            case 5:
            this.txtbox = ((System.Windows.Controls.TextBox)(target));
            
            #line 26 "..\..\HitSounds.xaml"
            this.txtbox.MouseEnter += new System.Windows.Input.MouseEventHandler(this.noDrag);
            
            #line default
            #line hidden
            
            #line 26 "..\..\HitSounds.xaml"
            this.txtbox.MouseLeave += new System.Windows.Input.MouseEventHandler(this.doDrag);
            
            #line default
            #line hidden
            return;
            case 6:
            
            #line 28 "..\..\HitSounds.xaml"
            ((System.Windows.Controls.Button)(target)).MouseEnter += new System.Windows.Input.MouseEventHandler(this.noDrag);
            
            #line default
            #line hidden
            
            #line 28 "..\..\HitSounds.xaml"
            ((System.Windows.Controls.Button)(target)).MouseLeave += new System.Windows.Input.MouseEventHandler(this.doDrag);
            
            #line default
            #line hidden
            
            #line 28 "..\..\HitSounds.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Install);
            
            #line default
            #line hidden
            return;
            case 7:
            
            #line 29 "..\..\HitSounds.xaml"
            ((System.Windows.Controls.Button)(target)).MouseEnter += new System.Windows.Input.MouseEventHandler(this.noDrag);
            
            #line default
            #line hidden
            
            #line 29 "..\..\HitSounds.xaml"
            ((System.Windows.Controls.Button)(target)).MouseLeave += new System.Windows.Input.MouseEventHandler(this.doDrag);
            
            #line default
            #line hidden
            
            #line 29 "..\..\HitSounds.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Choose);
            
            #line default
            #line hidden
            return;
            case 8:
            this.Sound = ((System.Windows.Controls.TextBox)(target));
            
            #line 30 "..\..\HitSounds.xaml"
            this.Sound.MouseEnter += new System.Windows.Input.MouseEventHandler(this.noDrag);
            
            #line default
            #line hidden
            
            #line 30 "..\..\HitSounds.xaml"
            this.Sound.MouseLeave += new System.Windows.Input.MouseEventHandler(this.doDrag);
            
            #line default
            #line hidden
            return;
            case 9:
            this.GoodHitSound = ((System.Windows.Controls.CheckBox)(target));
            
            #line 31 "..\..\HitSounds.xaml"
            this.GoodHitSound.Checked += new System.Windows.RoutedEventHandler(this.GoodHit);
            
            #line default
            #line hidden
            
            #line 31 "..\..\HitSounds.xaml"
            this.GoodHitSound.MouseEnter += new System.Windows.Input.MouseEventHandler(this.noDrag);
            
            #line default
            #line hidden
            
            #line 31 "..\..\HitSounds.xaml"
            this.GoodHitSound.MouseLeave += new System.Windows.Input.MouseEventHandler(this.doDrag);
            
            #line default
            #line hidden
            return;
            case 10:
            this.BadHitSounds = ((System.Windows.Controls.CheckBox)(target));
            
            #line 32 "..\..\HitSounds.xaml"
            this.BadHitSounds.Checked += new System.Windows.RoutedEventHandler(this.BadHit);
            
            #line default
            #line hidden
            
            #line 32 "..\..\HitSounds.xaml"
            this.BadHitSounds.MouseEnter += new System.Windows.Input.MouseEventHandler(this.noDrag);
            
            #line default
            #line hidden
            
            #line 32 "..\..\HitSounds.xaml"
            this.BadHitSounds.MouseLeave += new System.Windows.Input.MouseEventHandler(this.doDrag);
            
            #line default
            #line hidden
            return;
            case 11:
            this.MenuMusic = ((System.Windows.Controls.CheckBox)(target));
            
            #line 33 "..\..\HitSounds.xaml"
            this.MenuMusic.Checked += new System.Windows.RoutedEventHandler(this.Menu);
            
            #line default
            #line hidden
            
            #line 33 "..\..\HitSounds.xaml"
            this.MenuMusic.MouseEnter += new System.Windows.Input.MouseEventHandler(this.noDrag);
            
            #line default
            #line hidden
            
            #line 33 "..\..\HitSounds.xaml"
            this.MenuMusic.MouseLeave += new System.Windows.Input.MouseEventHandler(this.doDrag);
            
            #line default
            #line hidden
            return;
            case 12:
            this.MenuClickSound = ((System.Windows.Controls.CheckBox)(target));
            
            #line 34 "..\..\HitSounds.xaml"
            this.MenuClickSound.Checked += new System.Windows.RoutedEventHandler(this.MenuClick);
            
            #line default
            #line hidden
            
            #line 34 "..\..\HitSounds.xaml"
            this.MenuClickSound.MouseEnter += new System.Windows.Input.MouseEventHandler(this.noDrag);
            
            #line default
            #line hidden
            
            #line 34 "..\..\HitSounds.xaml"
            this.MenuClickSound.MouseLeave += new System.Windows.Input.MouseEventHandler(this.doDrag);
            
            #line default
            #line hidden
            return;
            case 13:
            this.FireWorks = ((System.Windows.Controls.CheckBox)(target));
            
            #line 35 "..\..\HitSounds.xaml"
            this.FireWorks.Checked += new System.Windows.RoutedEventHandler(this.Highscore);
            
            #line default
            #line hidden
            
            #line 35 "..\..\HitSounds.xaml"
            this.FireWorks.MouseEnter += new System.Windows.Input.MouseEventHandler(this.noDrag);
            
            #line default
            #line hidden
            
            #line 35 "..\..\HitSounds.xaml"
            this.FireWorks.MouseLeave += new System.Windows.Input.MouseEventHandler(this.doDrag);
            
            #line default
            #line hidden
            return;
            case 14:
            this.LevelCleared = ((System.Windows.Controls.CheckBox)(target));
            
            #line 36 "..\..\HitSounds.xaml"
            this.LevelCleared.Checked += new System.Windows.RoutedEventHandler(this.Cleared);
            
            #line default
            #line hidden
            
            #line 36 "..\..\HitSounds.xaml"
            this.LevelCleared.MouseEnter += new System.Windows.Input.MouseEventHandler(this.noDrag);
            
            #line default
            #line hidden
            
            #line 36 "..\..\HitSounds.xaml"
            this.LevelCleared.MouseLeave += new System.Windows.Input.MouseEventHandler(this.doDrag);
            
            #line default
            #line hidden
            return;
            case 15:
            
            #line 37 "..\..\HitSounds.xaml"
            ((System.Windows.Controls.Button)(target)).MouseEnter += new System.Windows.Input.MouseEventHandler(this.noDrag);
            
            #line default
            #line hidden
            
            #line 37 "..\..\HitSounds.xaml"
            ((System.Windows.Controls.Button)(target)).MouseLeave += new System.Windows.Input.MouseEventHandler(this.doDrag);
            
            #line default
            #line hidden
            
            #line 37 "..\..\HitSounds.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Reset);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

