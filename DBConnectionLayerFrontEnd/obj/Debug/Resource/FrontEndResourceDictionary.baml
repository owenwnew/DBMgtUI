   M S B A M L   `   `   `  ����T  PDBConnectionLayerFrontEnd, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullW0clr-namespace:DBConnectionLayerFrontEnd.Resource"DBConnectionLayerFrontEnd.Resource  5      Y1clr-namespace:DBConnectionLayerFrontEnd.ViewModel#DBConnectionLayerFrontEnd.ViewModel  5      O,clr-namespace:DBConnectionLayerFrontEnd.ViewDBConnectionLayerFrontEnd.View  5      Q-clr-namespace:DBConnectionLayerFrontEnd.ModelDBConnectionLayerFrontEnd.Model  5      W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089   Button_Background  �� EndPoint �� 
StartPoint � Color � Offset �� DataType>    8DBConnectionLayerFrontEnd.ViewModel.CustomerMgtViewModel4   .DBConnectionLayerFrontEnd.View.CustomerMgtView;   5DBConnectionLayerFrontEnd.ViewModel.OrderMgtViewModel1   +DBConnectionLayerFrontEnd.View.OrderMgtViewB   <DBConnectionLayerFrontEnd.ViewModel.InvoiceItemListViewModel8   2DBConnectionLayerFrontEnd.View.InvoiceItemListView  toolBarTemplate  MyFocusVisual � FocusVisualStyle � Cursor �� Path  InvoiceItemListGridTemplate �� Orientation&   System.Windows.Controls.DataGrid	 J� 	AllowDrop
   AlternatingRowBackground �� AlternationCount   RowDetailsVisibilityMode   EnableRowVirtualization   AutoGenerateColumns   CanUserAddRows   CanUserDeleteRows   Columns0  *System.Windows.Controls.DataGridTextColumn,  &System.Windows.Controls.DataGridColumn   Header   Width5	  /System.Windows.Controls.DataGridLengthConverter1
  +System.Windows.Controls.DataGridBoundColumn 
  Binding  ClosableTabItemTemplate  OrdersWorkspacesTemplate � RowDefinitions$ �� IsSynchronizedWithCurrentItem  WorkspacesTemplate �� UpdateSourceTrigger�� 5      D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    :x,http://schemas.microsoft.com/winfx/2006/xaml     5      :local0clr-namespace:DBConnectionLayerFrontEnd.Resource  5      8vm1clr-namespace:DBConnectionLayerFrontEnd.ViewModel  5      3vw,clr-namespace:DBConnectionLayerFrontEnd.View  5      4md-clr-namespace:DBConnectionLayerFrontEnd.Model  5      %�
  &	        (��     5      *,  +)(�� ?    5      *, +)(�� q    5      *, +)&	 �    7   &	 �    &	 �    &	 1    7  &	 	    7  �� 5      $  0.5,1&�64   $ 0.5,0&�6E   .��5   
   � $ DeepSkyBlue��6   $ 0.963Q�6+   � 5   
   $ 	AliceBlue��6   $ 0.177Q�6)   � 5   
   $ 	AliceBlue��6   $
 0.41Q�6)   � 5   
   $ 	#FF8DE0FF��6   $ 0.771Q�6)   5      �� 5      "   6   .R�5   
     5      �� 5      "  6   .R�5   
     5      �� 5      "  6   .R�5    
     5!      �� 5#      .R�5$   
   c� $��1q�6*   
���������6   �� ��5%      �� 5&      $  0.5,1&�6&   $ 0.5,0&�67   .��5'      � $ 
WhiteSmoke��6#   $ 1Q�66   � 5(      $ White��6#   $ 0.409Q�61   5)      ��5+      �� 5-      .R�5/      �� $��1,0,0,0q�6   $��Center=�51      8��  
���   �
����� �$��30��55      # �  56      $
 Hand��57      ��5/   .   �� $ Command�.��5:      �� $��5,0,5,0q�6$   $��Black"�6Y   ��65   �� $ DisplayName�6Y   5F      5G      5I      5K      �� 5O      .R�5P   
   �� $ Vertical=�6   .+�5Q        	 . 

 �����$ 2��6L   $ VisibleWhenSelected=�6a    .  .  $��Stretch=�6�   $��5,2,5,0q�6   
������� .  . ��6�   ��  5R        5S      $
 Item��6G   $
 Auto	 6U    6)   �� $
 item�6U     5T      $ Description��6N   $
 Auto	 6c    6)   �� $ description�6c     5U      $ Quantity��6K   $
 Auto	 6]    6)   �� $ quantity�6]     5V      $ 
Unit Price��6L   $
 Auto	 6`    6)   �� $ 	unitPrice�6`     5W      $ Total Price��6M   $
 Auto	 6b    6)   �� $ 
totalPrice�6b   5^      5_      5`      �� 5c      .R�5d   
   ]� $
��Auto��6   .+�5e      �� $��X��5g   	   $
 Hand��5h   	   $��Right=�5i   	   |�.  $��Courier)�5k   	   $��9(�5l   	   $
��Bold"�5m   	   $��5,1,5,0q�5n   	   $��0q�5o   	   $��Bottom=�5p   	   $��16��5q   	   $��16��6   ��5f   	   �� $ CloseCommand�5q      �� 5s      $��Center=�5u   	   ��5t   	   �� $ DisplayName�5u   	   5x      5z      �� 5|      .R�5}   
   �  5~      �� 5      $��* �6    �� 5�      $
��Auto �6    .+�5�      �� $��0��6   $
 TrueY�5�      8��  $��4q�5�      ��5�      �� 5�      5�      5�      �� 5�      .R�5�   
   �  5�      �� 5�      $��* �6    �� 5�      $
��Auto �6    .+�5�      �� $��0��6   $
 TrueY�5�      8��  $��4q�5�      ��5�      �� 5�      �� 5�      $��1��6   .@�5�      �� $��24��6   ��6$   �� $ CurrentStatus�$ PropertyChanged=�5�      5�      5�      5�      