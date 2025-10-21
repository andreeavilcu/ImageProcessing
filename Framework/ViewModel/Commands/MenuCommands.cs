using Emgu.CV;
using Emgu.CV.Structure;

using System.Windows;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;
using System.Collections.Generic;

using Framework.View;
using static Framework.Utilities.DataProvider;
using static Framework.Utilities.DrawingHelper;
using static Framework.Utilities.FileHelper;
using static Framework.Converters.ImageConverter;

using Algorithms.Sections;
using Algorithms.Tools;
using Algorithms.Utilities;
using System;
using System.Drawing;

namespace Framework.ViewModel
{
    public class MenuCommands : BaseVM
    {
        private readonly MainVM _mainVM;
        public List<System.Windows.Point> VectorOfMousePosition = new List<System.Windows.Point>();
        public MenuCommands(MainVM mainVM)
        {
            _mainVM = mainVM;
            VectorOfMousePosition = new List<System.Windows.Point>();
        }

        private ImageSource InitialImage
        {
            get => _mainVM.InitialImage;
            set => _mainVM.InitialImage = value;
        }

        private ImageSource ProcessedImage
        {
            get => _mainVM.ProcessedImage;
            set => _mainVM.ProcessedImage = value;
        }

        private double ScaleValue
        {
            get => _mainVM.ScaleValue;
            set => _mainVM.ScaleValue = value;
        }

        #region File

        #region Load grayscale image
        private RelayCommand _loadGrayscaleImageCommand;
        public RelayCommand LoadGrayscaleImageCommand
        {
            get
            {
                if (_loadGrayscaleImageCommand == null)
                    _loadGrayscaleImageCommand = new RelayCommand(LoadGrayscaleImage);
                return _loadGrayscaleImageCommand;
            }
        }

        private void LoadGrayscaleImage(object parameter)
        {
            Clear(parameter);

            string fileName = LoadFileDialog("Select a grayscale picture");
            if (fileName != null)
            {
                GrayInitialImage = new Image<Gray, byte>(fileName);
                InitialImage = Convert(GrayInitialImage);
            }
        }
        #endregion

        #region Load color image
        private ICommand _loadColorImageCommand;
        public ICommand LoadColorImageCommand
        {
            get
            {
                if (_loadColorImageCommand == null)
                    _loadColorImageCommand = new RelayCommand(LoadColorImage);
                return _loadColorImageCommand;
            }
        }

        private void LoadColorImage(object parameter)
        {
            Clear(parameter);

            string fileName = LoadFileDialog("Select a color picture");
            if (fileName != null)
            {
                ColorInitialImage = new Image<Bgr, byte>(fileName);
                InitialImage = Convert(ColorInitialImage);
            }
        }
        #endregion

        #region Save processed image
        private ICommand _saveProcessedImageCommand;
        public ICommand SaveProcessedImageCommand
        {
            get
            {
                if (_saveProcessedImageCommand == null)
                    _saveProcessedImageCommand = new RelayCommand(SaveProcessedImage);
                return _saveProcessedImageCommand;
            }
        }

        private void SaveProcessedImage(object parameter)
        {
            if (GrayProcessedImage == null && ColorProcessedImage == null)
            {
                MessageBox.Show("If you want to save your processed image, " +
                    "please load and process an image first!");
                return;
            }

            string imagePath = SaveFileDialog("image.jpg");
            if (imagePath != null)
            {
                GrayProcessedImage?.Bitmap.Save(imagePath, GetJpegCodec("image/jpeg"), GetEncoderParameter(Encoder.Quality, 100));
                ColorProcessedImage?.Bitmap.Save(imagePath, GetJpegCodec("image/jpeg"), GetEncoderParameter(Encoder.Quality, 100));
                Process.Start(imagePath);
            }
        }
        #endregion

        #region Exit
        private ICommand _exitCommand;
        public ICommand ExitCommand
        {
            get
            {
                if (_exitCommand == null)
                    _exitCommand = new RelayCommand(Exit);
                return _exitCommand;
            }
        }

        private void Exit(object parameter)
        {
            Application.Current.Shutdown();
        }
        #endregion

        #endregion

        #region Edit

        #region Remove drawn shapes from initial canvas
        private ICommand _removeInitialDrawnShapesCommand;
        public ICommand RemoveInitialDrawnShapesCommand
        {
            get
            {
                if (_removeInitialDrawnShapesCommand == null)
                    _removeInitialDrawnShapesCommand = new RelayCommand(RemoveInitialDrawnShapes);
                return _removeInitialDrawnShapesCommand;
            }
        }

        private void RemoveInitialDrawnShapes(object parameter)
        {
            RemoveUiElements(parameter as Canvas);
        }
        #endregion

        #region Remove drawn shapes from processed canvas
        private ICommand _removeProcessedDrawnShapesCommand;
        public ICommand RemoveProcessedDrawnShapesCommand
        {
            get
            {
                if (_removeProcessedDrawnShapesCommand == null)
                    _removeProcessedDrawnShapesCommand = new RelayCommand(RemoveProcessedDrawnShapes);
                return _removeProcessedDrawnShapesCommand;
            }
        }

        private void RemoveProcessedDrawnShapes(object parameter)
        {
            RemoveUiElements(parameter as Canvas);
        }
        #endregion

        #region Remove drawn shapes from both canvases
        private ICommand _removeDrawnShapesCommand;
        public ICommand RemoveDrawnShapesCommand
        {
            get
            {
                if (_removeDrawnShapesCommand == null)
                    _removeDrawnShapesCommand = new RelayCommand(RemoveDrawnShapes);
                return _removeDrawnShapesCommand;
            }
        }

        private void RemoveDrawnShapes(object parameter)
        {
            var canvases = (object[])parameter;
            RemoveUiElements(canvases[0] as Canvas);
            RemoveUiElements(canvases[1] as Canvas);
        }
        #endregion

        #region Clear initial canvas
        private ICommand _clearInitialCanvasCommand;
        public ICommand ClearInitialCanvasCommand
        {
            get
            {
                if (_clearInitialCanvasCommand == null)
                    _clearInitialCanvasCommand = new RelayCommand(ClearInitialCanvas);
                return _clearInitialCanvasCommand;
            }
        }

        private void ClearInitialCanvas(object parameter)
        {
            RemoveUiElements(parameter as Canvas);

            GrayInitialImage = null;
            ColorInitialImage = null;
            InitialImage = null;
        }
        #endregion

        #region Clear processed canvas
        private ICommand _clearProcessedCanvasCommand;
        public ICommand ClearProcessedCanvasCommand
        {
            get
            {
                if (_clearProcessedCanvasCommand == null)
                    _clearProcessedCanvasCommand = new RelayCommand(ClearProcessedCanvas);
                return _clearProcessedCanvasCommand;
            }
        }

        private void ClearProcessedCanvas(object parameter)
        {
            var canvas = parameter as Canvas;
            if (canvas == null)
            {
                //MessageBox.Show("Invalid canvas parameter.");
                return;
            }

            RemoveUiElements(canvas);
            GrayProcessedImage = null;
            ColorProcessedImage = null;
            ProcessedImage = null;
        }
        #endregion

        #region Closing all open windows and clear both canvases
        private ICommand _clearCommand;
        public ICommand ClearCommand
        {
            get
            {
                if (_clearCommand == null)
                    _clearCommand = new RelayCommand(Clear);
                return _clearCommand;
            }
        }

        private void Clear(object parameter)
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window != Application.Current.MainWindow)
                {
                    window.Close();
                }
            }

            ScaleValue = 1;

            var canvases = (object[])parameter;
            ClearInitialCanvas(canvases[0] as Canvas);
            ClearProcessedCanvas(canvases[1] as Canvas);
        }
        #endregion

        #endregion

        #region Tools

        #region Magnifier
        private ICommand _magnifierCommand;
        public ICommand MagnifierCommand
        {
            get
            {
                if (_magnifierCommand == null)
                    _magnifierCommand = new RelayCommand(Magnifier);
                return _magnifierCommand;
            }
        }

        private void Magnifier(object parameter)
        {
            if (MagnifierOn == true) return;
            if (VectorOfMousePosition.Count == 0)
            {
                MessageBox.Show("Please select an area first.");
                return;
            }

            MagnifierWindow magnifierWindow = new MagnifierWindow();
            magnifierWindow.Show();
        }

        #endregion

        #region Visualize color levels

        #region Row color levels
        private ICommand _rowColorLevelsCommand;
        public ICommand RowColorLevelsCommand
        {
            get
            {
                if (_rowColorLevelsCommand == null)
                    _rowColorLevelsCommand = new RelayCommand(RowColorLevels);
                return _rowColorLevelsCommand;
            }
        }

        private void RowColorLevels(object parameter)
        {
            if (RowColorLevelsOn == true) return;
            if (MouseClickCollection.Count == 0)
            {
                MessageBox.Show("Please select an area first!");
                return;
            }

            ColorLevelsWindow window = new ColorLevelsWindow(_mainVM, CLevelsType.Row);
            window.Show();
        }
        #endregion

        #region Column color levels
        private ICommand _columnColorLevelsCommand;
        public ICommand ColumnColorLevelsCommand
        {
            get
            {
                if (_columnColorLevelsCommand == null)
                    _columnColorLevelsCommand = new RelayCommand(ColumnColorLevels);
                return _columnColorLevelsCommand;
            }
        }

        private void ColumnColorLevels(object parameter)
        {
            if (ColumnColorLevelsOn == true) return;
            if (MouseClickCollection.Count == 0)
            {
                MessageBox.Show("Please select an area first!");
                return;
            }

            ColorLevelsWindow window = new ColorLevelsWindow(_mainVM, CLevelsType.Column);
            window.Show();
        }
        #endregion

        #endregion

        #region Visualize image histogram

        #region Initial image histogram
        private ICommand _histogramInitialImageCommand;
        public ICommand HistogramInitialImageCommand
        {
            get
            {
                if (_histogramInitialImageCommand == null)
                    _histogramInitialImageCommand = new RelayCommand(HistogramInitialImage);
                return _histogramInitialImageCommand;
            }
        }

        private void HistogramInitialImage(object parameter)
        {
            if (InitialHistogramOn == true) return;
            if (InitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            HistogramWindow window = null;

            if (ColorInitialImage != null)
            {
                window = new HistogramWindow(_mainVM, ImageType.InitialColor);
            }
            else if (GrayInitialImage != null)
            {
                window = new HistogramWindow(_mainVM, ImageType.InitialGray);
            }

            window.Show();
        }
        #endregion

        #region Processed image histogram
        private ICommand _histogramProcessedImageCommand;
        public ICommand HistogramProcessedImageCommand
        {
            get
            {
                if (_histogramProcessedImageCommand == null)
                    _histogramProcessedImageCommand = new RelayCommand(HistogramProcessedImage);
                return _histogramProcessedImageCommand;
            }
        }

        private void HistogramProcessedImage(object parameter)
        {
            if (ProcessedHistogramOn == true) return;
            if (ProcessedImage == null)
            {
                MessageBox.Show("Please process an image first!");
                return;
            }

            HistogramWindow window = null;

            if (ColorProcessedImage != null)
            {
                window = new HistogramWindow(_mainVM, ImageType.ProcessedColor);
            }
            else if (GrayProcessedImage != null)
            {
                window = new HistogramWindow(_mainVM, ImageType.ProcessedGray);
            }

            window.Show();
        }
        #endregion

        #endregion

        #region Copy image
        private ICommand _copyImageCommand;
        public ICommand CopyImageCommand
        {
            get
            {
                if (_copyImageCommand == null)
                    _copyImageCommand = new RelayCommand(CopyImage);
                return _copyImageCommand;
            }
        }

        private void CopyImage(object parameter)
        {
            if (InitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            ClearProcessedCanvas(parameter);

            if (ColorInitialImage != null)
            {
                ColorProcessedImage = Tools.Copy(ColorInitialImage);
                ProcessedImage = Convert(ColorProcessedImage);
            }
            else if (GrayInitialImage != null)
            {
                GrayProcessedImage = Tools.Copy(GrayInitialImage);
                ProcessedImage = Convert(GrayProcessedImage);
            }
        }
        #endregion

        #region binaryImage
        private ICommand _binaryImageCommand;
        public ICommand BinaryImageCommand
        {
            get
            {
                if (_binaryImageCommand == null)
                    _binaryImageCommand = new RelayCommand(BinaryImage);
                return _binaryImageCommand;
            }
        }

        private void BinaryImage(object parameter)
        {
            if (InitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            ClearProcessedCanvas(parameter);

            List<string> parameters = new List<string>()
            {
                "Threshhold (0-255)",
            };

            DialogWindow window = new DialogWindow(_mainVM, parameters);
            window.ShowDialog();
            List<double> values = window.GetValues();
            double threshold = values[0] + 0.5;
            threshold = Math.Min(255, Math.Max(0, threshold));


            if (GrayInitialImage != null)
            {
                GrayProcessedImage = Tools.Binary(GrayInitialImage, (byte)threshold);
                ProcessedImage = Convert(GrayProcessedImage);
            }
            else //if (ColorInitialImage != null)
            {
                GrayProcessedImage = Tools.Convert(ColorInitialImage);
                GrayProcessedImage = Tools.Binary(GrayProcessedImage,
               (byte)threshold);
                ProcessedImage = Convert(GrayProcessedImage);

            }
        }
        #endregion

        #region Rotate Clockwise
        private ICommand _rotateClockwiseCommand;
        public ICommand RotateClockwiseCommand
        {
            get
            {
                if (_rotateClockwiseCommand == null)
                    _rotateClockwiseCommand = new RelayCommand(RotateClockwise);
                return _rotateClockwiseCommand;
            }
        }

        private void RotateClockwise(object parameter)
        {
            if (InitialImage == null)
            {
                MessageBox.Show("Please load an image first.");
                return;
            }

            var canvas = parameter as Canvas;
            if (canvas != null)
            {
                ClearProcessedCanvas(canvas);
            }

            if (GrayInitialImage != null)
            {
                GrayProcessedImage = Tools.RotateClockwise(GrayInitialImage);
                ProcessedImage = Convert(GrayProcessedImage);
            }
            else if (ColorInitialImage != null)
            {
                ColorProcessedImage = Tools.RotateClockwise(ColorInitialImage);
                ProcessedImage = Convert(ColorProcessedImage);
            }
        }

        #endregion

        #region Rotate Anti-Clockwise
        private ICommand _rotateAntiClockwiseCommand;
        public ICommand RotateAntiClockwiseCommand
        {
            get
            {
                if (_rotateAntiClockwiseCommand == null)
                    _rotateAntiClockwiseCommand = new RelayCommand(RotateAntiClockwise);
                return _rotateAntiClockwiseCommand;
            }
        }

        private void RotateAntiClockwise(object parameter)
        {
            if (InitialImage == null)
            {
                MessageBox.Show("Please load an image first.");
                return;
            }

            var canvas = parameter as Canvas;
            if (canvas != null)
            {
                ClearProcessedCanvas(canvas);
            }

            if (GrayInitialImage != null)
            {
                GrayProcessedImage = Tools.RotateAntiClockwise(GrayInitialImage);
                ProcessedImage = Convert(GrayProcessedImage);
            }
            else if (ColorInitialImage != null)
            {
                ColorProcessedImage = Tools.RotateAntiClockwise(ColorInitialImage);
                ProcessedImage = Convert(ColorProcessedImage);
            }
        }
        #endregion


        #region Mirroring
        private ICommand _mirrorImageCommand;
        public ICommand MirrorImageCommand
        {
            get
            {
                if (_mirrorImageCommand == null)
                    _mirrorImageCommand = new RelayCommand(MirrorImage);
                return _mirrorImageCommand;
            }
        }

        private void MirrorImage(object parameter)
        {
            if (InitialImage == null)
            {
                MessageBox.Show("Please load an image first.");
                return;
            }

            
            var canvas = parameter as Canvas;
            if (canvas != null)
            {
                ClearProcessedCanvas(canvas);
            }
            else
            {
                 //MessageBox.Show("Canvas is null. Skipping canvas clearing.");
            }

            try
            {
                if (GrayInitialImage != null)
                {
                    var mirrored = Tools.Mirror(GrayInitialImage);
                    if (mirrored == null)
                        throw new Exception("Mirroring failed for grayscale image.");
                    GrayProcessedImage = mirrored;
                    ProcessedImage = Convert(GrayProcessedImage);
                }
                else if (ColorInitialImage != null)
                {
                    var mirrored = Tools.Mirror(ColorInitialImage);
                    if (mirrored == null)
                        throw new Exception("Mirroring failed for color image.");
                    ColorProcessedImage = mirrored;
                    ProcessedImage = Convert(ColorProcessedImage);
                }
                else
                {
                    MessageBox.Show("No valid image to mirror.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error during mirroring: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region Crop
        private ICommand _cropCommand;
        public ICommand CropCommand
        {
            get
            {
                if (_cropCommand == null)
                    _cropCommand = new RelayCommand(Crop);
                return _cropCommand;
            }
        }

        private System.Windows.Point _firstPoint;
        private System.Windows.Point _secondPoint;
        private bool _isFirstPointSelected = false;


        private void Crop(object parameter)
        {
            _mainVM.IsCropMode = true;
            VectorOfMousePosition.Clear();
            MessageBox.Show("Crop mode activated. Please select two points on the image.");
        }

        public void PerformCrop(Canvas canvas)
        {

            if (VectorOfMousePosition.Count >= 2)
            {

                var p1 = VectorOfMousePosition[0];
                var p2 = VectorOfMousePosition[1];


                var topLeft = new System.Windows.Point(
                    Math.Min(p1.X, p2.X),
                    Math.Min(p1.Y, p2.Y)
                );

                var bottomRight = new System.Windows.Point(
                    Math.Max(p1.X, p2.X),
                    Math.Max(p1.Y, p2.Y)
                );


                double zoom = _mainVM.ScaleValue;

                DrawRectangle(canvas, topLeft, bottomRight, 2, System.Windows.Media.Brushes.Red, zoom);

                try
                {
                    if (GrayInitialImage != null)
                    {
                        var p1Drawing = new System.Drawing.Point((int)p1.X, (int)p1.Y);
                        var p2Drawing = new System.Drawing.Point((int)p2.X, (int)p2.Y);
                        GrayProcessedImage = Tools.Crop(GrayInitialImage, p1Drawing, p2Drawing, zoom);
                        ProcessedImage = Convert(GrayProcessedImage);

                        var stats = Tools.CalculateStatistics(GrayProcessedImage);
                        MessageBox.Show($"Mean: {stats.mean:F2}\nStandard Deviation: {stats.standardDeviation:F2}", "Cropped Area Statistics");
                    }
                    else if (ColorInitialImage != null)
                    {
                        var p1Drawing = new System.Drawing.Point((int)p1.X, (int)p1.Y);
                        var p2Drawing = new System.Drawing.Point((int)p2.X, (int)p2.Y);
                        ColorProcessedImage = Tools.Crop(ColorInitialImage, p1Drawing, p2Drawing, zoom);
                        ProcessedImage = Convert(ColorProcessedImage);

                        var grayImage = ColorProcessedImage.Convert<Gray, byte>();
                        var stats = Tools.CalculateStatistics(grayImage);
                        MessageBox.Show($"Mean: {stats.mean:F2}\nStandard Deviation: {stats.standardDeviation:F2}", "Cropped Area Statistics");
                    }
                }
                catch (ArgumentException ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                _mainVM.IsCropMode = false;

                VectorOfMousePosition.Clear();
            }


        }



        #endregion

        #region Invert image
        private ICommand _invertImageCommand;
        public ICommand InvertImageCommand
        {
            get
            {
                if (_invertImageCommand == null)
                    _invertImageCommand = new RelayCommand(InvertImage);
                return _invertImageCommand;
            }
        }

        private void InvertImage(object parameter)
        {
            if (InitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            ClearProcessedCanvas(parameter as Canvas);

            if (GrayInitialImage != null)
            {
                List<string> label = new List<string>
                {
                    "Threshhold (0-255)"
                };

                DialogWindow dialog = new DialogWindow(_mainVM, label);
                dialog.ShowDialog();
                GrayProcessedImage = Tools.Invert(GrayInitialImage);
                ProcessedImage = Convert(GrayProcessedImage);
            }
            else if (ColorInitialImage != null)
            {
                ColorProcessedImage = Tools.Invert(ColorInitialImage);
                ProcessedImage = Convert(ColorProcessedImage);
            }
        }
        #endregion


        #region Convert color image to grayscale image
        private ICommand _convertImageToGrayscaleCommand;
        public ICommand ConvertImageToGrayscaleCommand
        {
            get
            {
                if (_convertImageToGrayscaleCommand == null)
                    _convertImageToGrayscaleCommand = new RelayCommand(ConvertImageToGrayscale);
                return _convertImageToGrayscaleCommand;
            }
        }

        private void ConvertImageToGrayscale(object parameter)
        {
            if (InitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            ClearProcessedCanvas(parameter);

            if (ColorInitialImage != null)
            {
                GrayProcessedImage = Tools.Convert(ColorInitialImage);
                ProcessedImage = Convert(GrayProcessedImage);
            }
            else
            {
                MessageBox.Show("It is possible to convert only color images!");
            }
        }
        #endregion

        #endregion


        #region Pointwise operations

        #region Contrast and Brightness

        private ICommand _contrastAndBrightnessCommand;

        public ICommand ContrastAndBrightnessCommand
        {
            get
            {
                if (_contrastAndBrightnessCommand == null)
                    _contrastAndBrightnessCommand = new RelayCommand(ContrastAndBrightness);
                return _contrastAndBrightnessCommand;
            }
        }

        private void ContrastAndBrightness(object parameter)
        {
            if (InitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            ClearProcessedCanvas(parameter);

            List<string> labels = new List<string>()
            {
                "Alpha (Contrast, > 0)",
                "Beta (Brightness)"
            };

            DialogWindow window = new DialogWindow(_mainVM, labels);
            window.ShowDialog();

            List<double> values = window.GetValues();

            if (values == null || values.Count != 2)
            {
                return;
            }

            double alpha = values[0];
            double beta = values[1];

            if (alpha <= 0)
            {
                MessageBox.Show("Alpha must be a strictly positive value!", "Validation Error");
                return;
            }

            if (ColorInitialImage != null)
            {
                ColorProcessedImage = PointwiseOperations.Brightness(ColorInitialImage, alpha, beta);
                ProcessedImage = Convert(ColorProcessedImage);
            }
            else if (GrayInitialImage != null)
            {
                GrayProcessedImage = PointwiseOperations.Brightness(GrayInitialImage, alpha, beta);
                ProcessedImage = Convert(GrayProcessedImage);
            }
        }

        #endregion

        #region Gamma
        private ICommand _gammaOperatorCommand;

        public ICommand GammaOperatorCommand
        {
            get
            {
                if (_gammaOperatorCommand == null)
                    _gammaOperatorCommand = new RelayCommand(GammaBrightness);
                return _gammaOperatorCommand;
            }
        }

        private void GammaBrightness(object parameter)
        {
            if (InitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            ClearProcessedCanvas(parameter);

            List<string> labels = new List<string>()
            {
                "Gamma "

            };

            DialogWindow window = new DialogWindow(_mainVM, labels);
            window.ShowDialog();

            List<double> values = window.GetValues();

            if (values == null || values.Count != 1)
            {
                return;
            }

            double gamma = values[0];


            if (gamma <= 0)
            {
                MessageBox.Show("Gamma must be a strictly positive value!", "Validation Error");
                return;
            }

            if (ColorInitialImage != null)
            {
                ColorProcessedImage = PointwiseOperations.GammaOperator(ColorInitialImage, gamma);
                ProcessedImage = Convert(ColorProcessedImage);
            }
            else if (GrayInitialImage != null)
            {
                GrayProcessedImage = PointwiseOperations.GammaOperator(GrayInitialImage, gamma);
                ProcessedImage = Convert(GrayProcessedImage);
            }
        }

        #endregion

        #endregion



        #region Thresholding

        #region MinErr Threshold
        private ICommand _minErrThresholdCommand;
        public ICommand MinErrThresholdCommand
        {
            get
            {
                if (_minErrThresholdCommand == null)
                    _minErrThresholdCommand = new RelayCommand(MinErrThreshold);
                return _minErrThresholdCommand;
            }
        }

        #region Thresholding
        private void MinErrThreshold(object parameter)
        {
            if (InitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            ClearProcessedCanvas(parameter as Canvas);


            IImage processedImage = null;

            if (GrayInitialImage != null)
            {
                processedImage = Thresholding.MinErrThreshold2(GrayInitialImage);
                GrayProcessedImage = processedImage as Image<Gray, byte>;
                ProcessedImage = Convert(GrayProcessedImage);
            }
            else if (ColorInitialImage != null)
            {
                processedImage = Thresholding.MinErrThreshold2(ColorInitialImage);
                GrayProcessedImage = processedImage as Image<Gray, byte>;
                ProcessedImage = Convert(GrayProcessedImage);
            }
            else
            {
                MessageBox.Show("No image loaded for thresholding.");
            }


            if (processedImage is Image<Gray, byte> || processedImage is Image<Bgr, byte>)
            {

            }
            else if (processedImage != null)
            {
                processedImage.Dispose();
            }
        }
        #endregion
        #endregion
        #endregion

        #region Geometric transformations
        #endregion



        #region Filters
        #endregion

        #region Morphological operations
        #endregion

        #region Geometric transformations
        #endregion

        #region Segmentation
        #endregion

        #region Use processed image as initial image
        private ICommand _useProcessedImageAsInitialImageCommand;
        public ICommand UseProcessedImageAsInitialImageCommand
        {
            get
            {
                if (_useProcessedImageAsInitialImageCommand == null)
                    _useProcessedImageAsInitialImageCommand = new RelayCommand(UseProcessedImageAsInitialImage);
                return _useProcessedImageAsInitialImageCommand;
            }
        }

        private void UseProcessedImageAsInitialImage(object parameter)
        {
            if (ProcessedImage == null)
            {
                MessageBox.Show("Please process an image first!");
                return;
            }

            var canvases = (object[])parameter;

            ClearInitialCanvas(canvases[0] as Canvas);

            if (GrayProcessedImage != null)
            {
                GrayInitialImage = GrayProcessedImage;
                InitialImage = Convert(GrayInitialImage);
            }
            else if (ColorProcessedImage != null)
            {
                ColorInitialImage = ColorProcessedImage;
                InitialImage = Convert(ColorInitialImage);
            }

            ClearProcessedCanvas(canvases[1] as Canvas);
        }
        #endregion

       
    }
}