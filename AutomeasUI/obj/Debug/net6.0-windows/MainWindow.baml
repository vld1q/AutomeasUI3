   M S B A M L   `   `   `  ����E  AAutomeasUI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null1clr-namespace:AutomeasUI.ViewAutomeasUI.View  5   	       AutomeasUI.MainWindow   5   	   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   .viewclr-namespace:AutomeasUI.View     5   	   -   5        $
��Root��5   	     � Title$  
MainWindow��5	   	   $	��450��6   $	��800��6)   .��5
      � $��10,0,0,0q�6    � ColumnDefinitions 5   
   �� 5      $
��0.4* �6   �� 5      $��* �6   .+�5   
   � $��0��6   .+�5      #   AutomeasUI.View.ConfigBarView  5      � 5   
   $��1��6   .+�5      #   AutomeasUI.View.DashboardView   � DataContext 6!   �� *DashboardViewModel+ �� ElementName$
 Root��5      5      5      