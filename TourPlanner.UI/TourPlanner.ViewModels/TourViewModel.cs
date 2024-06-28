﻿using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Tour_planner.TourPlanner.BusinessLayer.TourPlanner.Services;
using Tour_planner.TourPlanner.UI.TourPlanner.Models;
using Tour_planner.TourPlanner.Commands;
using System.Linq;
using log4net;
using Tour_planner.TourPlanner.UI.TourPlanner.Views;

namespace Tour_planner.TourPlanner.UI.TourPlanner.ViewModels {
    public class TourViewModel : ViewModelBase {

        private static readonly ILog log = LogManager.GetLogger(typeof(TourViewModel));
        private readonly TourService _tourService;
        private readonly OpenRouteService _routeService;
        private readonly MainWindow _mainWindow;

        public ObservableCollection<Tour> Tours { get; private set; }
        private Tour _selectedTour;
        public Tour SelectedTour {
            get => _selectedTour;
            set {
                if (SetProperty(ref _selectedTour, value) && value != null)
                {
                    //Update fields with selected tour details
                    NewTourName = value.TourName;
                    NewTourDescription = value.Description;
                    NewTourFrom = value.From;
                    NewTourTo = value.To;
                    NewTourTransportType = value.TransportType;
                    NewTourTransportType = value.TransportType;
                    //NewTourDistance = value.TourDistance;
                    //NewTourEstimatedTime = value.EstimatedTime;
                    //NewTourPopularity = value.Popularity;
                    //NewTourChildFriendliness = value.ChildFriendliness;

                    //Display route on map
                    _ = DisplayRouteOnMap(value.From, value.To);
                }   

                (UpdateTourCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (DeleteTourCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        // Properties for new tour input
        public string NewTourName { get; set; }
        public string NewTourDescription { get; set; }
        public string NewTourFrom { get; set; }
        public string NewTourTo { get; set; }
        public string NewTourTransportType { get; set; }

        // Commands
        public ICommand AddTourCommand { get; }
        public ICommand UpdateTourCommand { get; }
        public ICommand DeleteTourCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand GenerateTourReportCommand { get; }
        public ICommand GenerateSummaryReportCommand { get; }

        public string SearchText { get; set; }

        public TourViewModel(TourService tourService, OpenRouteService routeService, MainWindow mainWindow) {

            log.Debug("Initializing TourViewModel");

            _tourService = tourService;
            _routeService = routeService;
            _mainWindow = mainWindow;
            Tours = new ObservableCollection<Tour>(_tourService.GetAllTours());

            AddTourCommand = new RelayCommand(param => AddTour());
            UpdateTourCommand = new RelayCommand(param => UpdateTour(), param => CanModifyTour());
            DeleteTourCommand = new RelayCommand(param => DeleteTour(), param => CanModifyTour());
            SearchCommand = new RelayCommand(param => SearchTours());
            GenerateTourReportCommand = new RelayCommand(param => GenerateTourReport(), param => CanModifyTour());
            GenerateSummaryReportCommand = new RelayCommand(param => GenerateSummaryReport());
        }

        private async Task DisplayRouteOnMap(string from, string to)
        {
            try
            {
                var fromCoordinates = await _routeService.GetCoordinatesAsync(from);
                var toCoordinates = await _routeService.GetCoordinatesAsync(to);

                var route = await _routeService.GetRouteAsync(
                    $"{fromCoordinates.Longitude}, {fromCoordinates.Latitude}",
                    $"{toCoordinates.Longitude}, {toCoordinates.Latitude}"
                );
                _mainWindow.AddRouteToMap(route.ToString());
            }
            catch (Exception ex)
            {
                log.Error($"Error displaying route on map: {ex.Message}");
            }
        }

        private void AddTour() {

            log.Info($"Adding new tour: {NewTourName}");

            var newTour = new Tour {
                TourName = NewTourName,
                Description = NewTourDescription,
                From = NewTourFrom,
                To = NewTourTo,
                TransportType = NewTourTransportType
            };

            _tourService.AddTour(newTour);
            Tours.Add(newTour);
            ClearNewTourFields();
            log.Info("New tour added.");
        }

        private void UpdateTour() {
            if (SelectedTour != null) {
                log.Info($"Updating tour: {SelectedTour.TourName}");
                SelectedTour.TourName = NewTourName;
                SelectedTour.Description = NewTourDescription;
                SelectedTour.From = NewTourFrom;
                SelectedTour.To = NewTourTo;
                SelectedTour.TransportType = NewTourTransportType;

                _tourService.UpdateTour(SelectedTour);
                RefreshTours();
                log.Info("Tour updated.");
            }
        }

        private void DeleteTour() {
            if (SelectedTour != null) {
                log.Info($"Deleting tour: {SelectedTour.TourName}");
                _tourService.DeleteTour(SelectedTour);
                Tours.Remove(SelectedTour);
                SelectedTour = null;
                log.Info("Tour deleted.");
            }
        }

        private bool CanModifyTour() => SelectedTour != null;

        private void SearchTours() {
            log.Info($"Searching tours with text: {SearchText}");
            var searchResults = _tourService.SearchTours(SearchText);
            Tours.Clear();
            foreach (var tour in searchResults) {
                Tours.Add(tour);
            }
            log.Info("Tours search completed.");
        }

        private void RefreshTours() {
            Tours.Clear();
            foreach (var tour in _tourService.GetAllTours()) {
                Tours.Add(tour);
            }
        }

        private void ClearNewTourFields() {
            NewTourName = string.Empty;
            NewTourDescription = string.Empty;
            NewTourFrom = string.Empty;
            NewTourTo = string.Empty;
            NewTourTransportType = string.Empty;
            OnPropertyChanged(nameof(NewTourName));
            OnPropertyChanged(nameof(NewTourDescription));
            OnPropertyChanged(nameof(NewTourFrom));
            OnPropertyChanged(nameof(NewTourTo));
            OnPropertyChanged(nameof(NewTourTransportType));
        }

        private async void GenerateTourReport() {
            if (SelectedTour != null) {
                string outputPath = $"TourReport_{SelectedTour.TourId}.pdf";
                await _tourService.GenerateTourReport(SelectedTour.TourId, outputPath);
                log.Info($"Tour report generated for tour ID: {SelectedTour.TourId} at {outputPath}");
            }
        }

        private async void GenerateSummaryReport() {
            string outputPath = "SummaryReport.pdf";
            await _tourService.GenerateSummaryReport(outputPath);
            log.Info($"Summary report generated at {outputPath}");
        }
    }
}