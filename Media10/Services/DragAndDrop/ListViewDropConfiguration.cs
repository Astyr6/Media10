﻿using System;

using Media10.Models;

using Windows.UI.Xaml;

namespace Media10.Services.DragAndDrop
{
    public class ListViewDropConfiguration : DropConfiguration
    {
        public static readonly DependencyProperty DragItemsStartingActionProperty =
            DependencyProperty.Register("DragItemsStartingAction", typeof(Action<DragDropStartingData>), typeof(DropConfiguration), new PropertyMetadata(null));

        public static readonly DependencyProperty DragItemsCompletedActionProperty =
            DependencyProperty.Register("DragItemsCompletedAction", typeof(Action<DragDropCompletedData>), typeof(DropConfiguration), new PropertyMetadata(null));

        public Action<DragDropStartingData> DragItemsStartingAction
        {
            get => (Action<DragDropStartingData>)GetValue(DragItemsStartingActionProperty);
            set => SetValue(DragItemsStartingActionProperty, value);
        }

        public Action<DragDropCompletedData> DragItemsCompletedAction
        {
            get => (Action<DragDropCompletedData>)GetValue(DragItemsCompletedActionProperty);
            set => SetValue(DragItemsCompletedActionProperty, value);
        }
    }
}
