﻿using System;
using System.Collections.Generic;
using System.Windows;

namespace ConvergenceEngine.ViewModels.AppWindows {

    using Helpers;
    using Models;

    public sealed class MainWindowViewModel : CommandsViewModel {

        private DateTime lastTimeOfFrameUpdate;
        private TimeSpan frameUpdateLimit;

        private IEnumerable<Point> mapViewportData;

        public IEnumerable<Point> MapViewportData {
            get { return mapViewportData; }
            set { Set(ref mapViewportData, value); }
        }

        private ViewModelBase coloredDepthDataWindowViewModel;
        private ViewModelBase mixedDataWindowViewModel;
        private ViewModelBase motionDataWindowViewModel;

        public MainWindowViewModel() {
            model = new Model(Update);
            Initialize();
        }

        private void Initialize() {
            lastTimeOfFrameUpdate = DateTime.Now;
            frameUpdateLimit = TimeSpan.FromMilliseconds(1000.0 / 29.97);
            InitializeData();
            CreateViewModelsForChildWindows();
            InitializeCommands();
        }

        private void CreateViewModelsForChildWindows() {
            coloredDepthDataWindowViewModel = new ColoredDepthDataWindowViewModel(model);
            mixedDataWindowViewModel = new MixedDataWindowViewModel(model.Map);
            motionDataWindowViewModel = new MotionDataWindowViewModel(model.Map);
        }

        protected override void InitializeCommands() {
            base.InitializeCommands();

            ShowRawDataWindow = new RelayCommand(
                ex => ExecuteNewWindowCommand(coloredDepthDataWindowViewModel),
                canEx => CanExecuteNewWindowCommand(coloredDepthDataWindowViewModel));

            ShowMixedDataWindow = new RelayCommand(
                ex => ExecuteNewWindowCommand(mixedDataWindowViewModel),
                canEx => CanExecuteNewWindowCommand(mixedDataWindowViewModel));

            ShowMotionDataWindow = new RelayCommand(
                ex => ExecuteNewWindowCommand(motionDataWindowViewModel),
                canEx => CanExecuteNewWindowCommand(motionDataWindowViewModel));
        }

        private void Update() {

            ModelReady = model.Ready;

            if ((DateTime.Now - lastTimeOfFrameUpdate) >= frameUpdateLimit && ModelReady) {
                IEnumerable<Point> mapPoints = model.Map.GetMapPoints();
                if (mapPoints != null) {
                    MapViewportData = mapPoints;
                    lastTimeOfFrameUpdate = DateTime.Now;
                }
            }            
        }
    }
}