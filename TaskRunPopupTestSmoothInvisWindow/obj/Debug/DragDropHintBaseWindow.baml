   M S B A M L   `   `   `  ����K  GGiveFeedbackTest, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null3clr-namespace:GiveFeedbackTestGiveFeedbackTest  5   	   -    'GiveFeedbackTest.DragDropHintBaseWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   0localclr-namespace:GiveFeedbackTest     5   	     � AllowsTransparency  .  � WindowStyle$
 None=�5	   	    � Title$ DragDropHintBaseWindow��5
   	   $	��450��6(   $	��800��65   ���	#77FFFFFF$y�Hidden=�5   	    � 	Resources 5         PopupStyle1 �� 
TargetType �� Property �� Value��%F  &	        �� 5   
   " �6$   .�5      ��  �@�AllowsTransparency . 63   �� 5       � ��$ Center=�6*   �� 5       �@J�SnapsToDevicePixels .  64   �� 5       �@�UseLayoutRounding . 62   �� 5       � ��$
 Fade=�6/   �� 5       �@J�Opacity$ 1Q�6(   5      5      .��5      � .+�5   
   � -   5        $��myPopup2��5      $��40��6    #��[  5       J� IsHitTestVisible .   � AllowsTransparency . $��AbsolutePoint=�5      ��5      �� 	 �� ElementName$	 PopupStackPanel��
 �� Path$
 MaxWidth�.��5%      �� -   5         ��Name$ PopupStackPanel��5%      $	��160��62    .  .+�5&      �� -   5        $��myPopupImage��5&      $	��100��6,    .  6X   �� 5'      -   5        $��myPopupText��5'      
������  �� TextAlignment$ Center=�6H   $
��Wrap=�6_   
����  � �� Padding$ 	10,0,10,0q�6�    .  
Popup Text5)      5*      5+      5,      5-      