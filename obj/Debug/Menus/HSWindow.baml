   M S B A M L   `   `   `  ����  BodyBasics-WPF8    2Microsoft.Samples.Kinect.BodyBasics.Menus.HSWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   -   5          � Title$  	Tetris 3D��5   	   $	��750��6   $	��950��6(    � 
ResizeMode$ NoResize=�64   ��5      �� 5   
    �� ImageSource$ wallpaperMain.jpg��6   .��5      � .+�5	   
   � $��0,150,0,86.8q�6   $��3��6%    � ColumnDefinitions 5
      �� 5      $	��472 �6#   �� 5      $��414.4 �6#    � RowDefinitions 5      �� 5      $	��112 �6    �� 5      $	��100 �6    �� 5      $��52 �6    .+�5      �� $��1��6   $��0��6    $��0��6-   $
��Left=�6>   $	��Top=�6Y   $��10,0,0,0q�6q   $��25(�6�   
�������$��43��6�   $��34��6�   �� 5      $��2��6   $��1��6    $��0��6-   $
��Left=�6>   $	��Top=�6Y   $��
10,1.2,0,0q�6q   $��25(�6�   
�������$��43��6�   $��34��6�   �� 5      $��3��6   $��2��6    $��0��6-   $
��Left=�6>   $	��Top=�6Y   $��
10,2.4,0,0q�6q   $��25(�6�   
�������$��43��6�   $��34��6�   �� 5      $��4��6   $��0��6    $��1��6-   $
��Left=�6>   $	��Top=�6Y   $��
10.2,0,0,0q�6q   $��25(�6�   
�������$��43��6�   $��33��6�   �� 5      $��5��6   $��1��6    $��1��6-   $
��Left=�6>   $	��Top=�6Y   $��10.2,1.2,0,0q�6q   $��25(�6�   
�������$��43��6�   $��33��6�   �� 5      $��6��6   $��2��6    $��1��6-   $
��Left=�6>   $	��Top=�6Y   $��10.2,2.4,0,0q�6q   $��25(�6�   
�������$��43��6�   $��33��6�   �� 5      -   5        $��foto1��5      $��0��6!   $��0��6.   $
��Left=�6>   $	��Top=�6Y   $��78��6q   $��78��6}   $��50,0,0,0q�6�   �� 5      -   5        $��foto2��5      $��1��6!   $��0��6.   $
��Left=�6>   $	��Top=�6Y   $��78��6q   $��78��6}   $��
50,1.2,0,0q�6�   �� 5      -   5        $��foto3��5      $��2��6!   $��0��6.   $
��Left=�6>   $	��Top=�6Y   $��78��6q   $��78��6}   $��50,2.4,0,-27.6q�6�   �� 5      -   5        $��foto4��5      $��0��6!   $��1��6.   $
��Left=�6>   $	��Top=�6Y   $��78��6q   $��78��6}   $��
50.2,0,0,0q�6�   �� 5      -   5        $��foto5��5      $��1��6!   $��1��6.   $
��Left=�6>   $	��Top=�6Y   $��78��6q   $��78��6}   $��50.2,1.2,0,0q�6�   �� 5      -   5        $��foto6��5      $��2��6!   $��1��6.   $
��Left=�6>   $	��Top=�6Y   $��78��6q   $��78��6}   $��50.2,2.4,0,-27.6q�6�   �� 5       -   5        $��score0��5       $��score��6"   $��0��62   $��0��6?   $
��Left=�6P   $	��Top=�6k   $��	160,0,0,0q�6�   $��35(�6�   
�������$	��100��6�   $��56��6�   �� 5!      -	   5        $��score1��5!      $��score��6"   $��1��62   $��0��6?   $
��Left=�6P   $	��Top=�6k   $��160,1.2,0,0q�6�   $��35(�6�   
�������$	��100��6�   $��56��6�   �� 5"      -
   5        $��score2��5"      $��score��6"   $��2��62   $��0��6?   $
��Left=�6P   $	��Top=�6k   $��160,2.4,0,-5.6q�6�   $��35(�6�   
�������$	��100��6�   $��56��6�   �� 5#      -   5        $��score3��5#      $��score��6"   $��0��62   $��1��6?   $
��Left=�6P   $	��Top=�6k   $��160.2,0,0,0q�6�   $��35(�6�   
�������$	��100��6�   $��56��6�   �� 5$      -   5        $��score4��5$      $��score��6"   $��1��62   $��1��6?   $
��Left=�6P   $	��Top=�6k   $��160.2,1.2,0,0q�6�   $��35(�6�   
�������$	��100��6�   $��56��6�   �� 5%      -   5        $��score5��5%      $��score��6"   $��2��62   $��1��6?   $
��Left=�6P   $	��Top=�6k   $��160.2,2.4,0,-5.6q�6�   $��35(�6�   
�������$	��100��6�   $��56��6�   �� 5'      -   5        $��data0��5'      $��
xx.xx.xxxx��6!   $��0��66   $��0��6C   $
��Left=�6T   $	��Top=�6o   $��
160,45,0,0q�6�   
�������$��20(�6�   $	��100��6�   $��35��6�   �� 5(      -   5        $��data1��5(      $��
xx.xx.xxxx��6!   $��1��66   $��0��6C   $
��Left=�6T   $	��Top=�6o   $��160,46.2,0,0q�6�   
�������$��20(�6�   $	��100��6�   $��35��6�   �� 5)      -   5        $��data2��5)      $��
xx.xx.xxxx��6!   $��2��66   $��0��6C   $
��Left=�6T   $	��Top=�6o   $��160,47.4,0,-29.6q�6�   
�������$��20(�6�   $	��100��6�   $��35��6�   �� 5*      -   5        $��data3��5*      $��
xx.xx.xxxx��6!   $��0��66   $��1��6C   $
��Left=�6T   $	��Top=�6o   $��160.2,45,0,0q�6�   
�������$��20(�6�   $	��100��6�   $��35��6�   �� 5+      -   5        $��data4��5+      $��
xx.xx.xxxx��6!   $��1��66   $��1��6C   $
��Left=�6T   $	��Top=�6o   $��160.2,46.2,0,0q�6�   
�������$��20(�6�   $	��100��6�   $��35��6�   �� 5,      -   5        $��data5��5,      $��
xx.xx.xxxx��6!   $��2��66   $��1��6C   $
��Left=�6T   $	��Top=�6o   $��160.2,47.4,0,-29.6q�6�   
�������$��20(�6�   $	��100��6�   $��35��6�   5.      �� 5/   
   $�� Highscores��6   $��0��6&   $��0��63   $��Center=�6D   $��Center=�6a   $��250,33,239,570.8q�6|   $��80(�6�   
�������$	��117��6�   $	��455��6�   �� 50   
   $
��Back��6   $��0��6    $��Center=�60   $��Center=�6M   $��402,504,384,68.8q�6i   $	��158��6�   $	��148��6�   $��35(�6�   $
��Bold"�6�   
�������   buttonStyle#��[  6�   �� 52   
   -   5         ��Name$ cursor��52      $	��100��6    $
��Fill=�6,   $��250,485,594,145.8q�6;   ��53      �� 54       �� 	UriSource$ hand.png?�6   56      58      59      