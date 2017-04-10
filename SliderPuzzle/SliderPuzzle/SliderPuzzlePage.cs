using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SliderPuzzle
{
    public class SliderPuzzlePage : ContentPage
    {
        // Properties
        private const int SIZE = 4;

        private AbsoluteLayout _absoluteLayout;

        private Dictionary<GridPosition, GridItem> _gridItems;

        public SliderPuzzlePage()
        {
            _gridItems = new Dictionary<GridPosition, GridItem>();
            _absoluteLayout = new AbsoluteLayout
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };
            var counter = 1;
            for (var row = 0; row < SIZE; row ++)
            {
                for (var col = 0; col < SIZE; col ++)
                {
                    GridItem item;

                    if(counter == 16)
                    {
                        item = new GridItem(new GridPosition(row, col), "empty");
                        item.FinalImage = "empty";
                    }
                    else
                    {
                        item = new GridItem(new GridPosition(row, col), counter.ToString());
                    }

                    var tapRecognizer = new TapGestureRecognizer();
                    tapRecognizer.Tapped += OnLabelTapped;
                    item.GestureRecognizers.Add(tapRecognizer);

                    _gridItems.Add(item.CurrentPosition, item);
                    _absoluteLayout.Children.Add(item);

                    counter++;
                }
            }

            Shuffle();

            ContentView contentView = new ContentView
            {
                Content = _absoluteLayout
            };
            contentView.SizeChanged += OnContentViewSizeChanged;
            this.Padding = new Thickness(5, Device.OnPlatform(25, 5, 5), 5, 5);
            this.Content = contentView;
        }

        private void OnLabelTapped(object sender, EventArgs e)
        {
            GridItem item = (GridItem)sender;

            if (item.isEmptySpot() == true)
            {
                return;
            }

            Random rand = new Random();
            int move = rand.Next(0, 4);

            if(move == 0 && item.CurrentPosition.Row == 0)
            {
                move = 2;
            }
            else if(move == 1 && item.CurrentPosition.Column == SIZE -1)
            {
                move = 3;
            }
            else if(move == 2 && item.CurrentPosition.Row == SIZE - 1)
            {
                move = 0;
            }
            else if(move == 3 && item.CurrentPosition.Column == 0)
            {
                move = 1;
            }

            int row = 0;
            int col = 0;

            if(move == 0)
            {
                row = item.CurrentPosition.Row - 1;
                col = item.CurrentPosition.Column;
            }
            else if(move == 1)
            {
                row = item.CurrentPosition.Row;
                col = item.CurrentPosition.Column + 1;
            }
            else if(move == 2)
            {
                row = item.CurrentPosition.Row + 1;
                col = item.CurrentPosition.Column;
            }
            else
            {
                row = item.CurrentPosition.Row;
                col = item.CurrentPosition.Column - 1;
            }

            GridItem swapWith = _gridItems[new GridPosition(row, col)];
            Swap(item, swapWith);
            OnContentViewSizeChanged(this.Content, null);

        }

        void OnContentViewSizeChanged(object sender, EventArgs args)
        {
            ContentView contentView = (ContentView)sender;
            double squareSize = Math.Min(contentView.Width, contentView.Height) / 4;

            for(var row = 0; row < SIZE; row++)
            {
                for(var col = 0; col < SIZE; col++)
                {
                    GridItem item = _gridItems[new GridPosition(row, col)];
                    Rectangle rect = new Rectangle(col * squareSize, row *
                        squareSize, squareSize, squareSize);
                    AbsoluteLayout.SetLayoutBounds(item, rect);
                }
            }
        }

        void Swap(GridItem item1, GridItem item2)
        {
            GridPosition temp = item1.CurrentPosition;
            item1.CurrentPosition = item2.CurrentPosition;
            item2.CurrentPosition = temp;

            _gridItems[item1.CurrentPosition] = item1;
            _gridItems[item2.CurrentPosition] = item2;
        }

        void Shuffle()
        {
            Random rand = new Random();
            for(var row = 0; row < SIZE; row++)
            {
                for (var col = 0; col < SIZE; col++)
                {
                    GridItem item = _gridItems[new GridPosition(row, col)];

                    int swapRow = rand.Next(0, 4);
                    int swapCol = rand.Next(0, 4);
                    GridItem swapItem = _gridItems[new GridPosition(swapRow, swapCol)];

                    Swap(item, swapItem);
                }
            }
        }

        internal class GridItem : Image
        {
            public GridPosition CurrentPosition
            {
                get; set;
            }
            public String FinalImage
            {
                get; set;
            }

            private GridPosition _finalPosition;
            private Boolean _isEmptySpot;

            
            public GridItem(GridPosition position, string text)
            {
                _finalPosition = position;
                CurrentPosition = position;
                if(text.Equals("empty"))
                {
                    _isEmptySpot = true;
                }
                else
                {
                    _isEmptySpot = false;
                }
                Source = ImageSource.FromResource("SliderPuzzle.house" + text + ".jpeg");
                HorizontalOptions = LayoutOptions.Center;
                VerticalOptions = LayoutOptions.Center;
            }

            public Boolean isEmptySpot()
            {
                return _isEmptySpot;
            }

            public void showFinalImage()
            {
                if(isEmptySpot())
                {
                    
                }
            }
        }

        internal class GridPosition
        {
            public int Row
            {
                get; set;
            }

            public int Column
            {
                get; set;
            }
            public GridPosition(int row, int col)
            {
                Row = row;
                Column = col;
            }
            public override bool Equals(object obj)
            {
                GridPosition other = obj as GridPosition;
                if (other != null && this.Row == other.Row && this.Column == other.Column)
                {
                    return true;
                }

                return false;
            }
            public override int GetHashCode()
            {
                return 17 * (23 + this.Row.GetHashCode()) * (23 + this.Column.GetHashCode());
            }
        }
    }
}
