class PlaceSelector {
    constructor() {
        this.initialize();
    }

    initialize() {
        $('#placeType').change(() => this.handlePlaceTypeChange());
        $('#country').change(() => this.handleCountryChange());
        $('#city').change(() => this.handleCityChange());

    }

    handlePlaceTypeChange() {
        const placeType = $('#placeType').val();
        if (placeType === 'online') {
            $('#onlineFields').show();
            $('#offlineFields').hide();
            this.loadOnlinePlaces();
        } else {
            $('#offlineFields').show();
            $('#onlineFields').hide();
            this.loadCountries();
        }
    }

    handleCountryChange() {
        const countryId = $('#country').val();
        this.loadCities(countryId);
    }

    handleCityChange() {
        const cityId = $('#city').val();
        this.loadPlacesInCity(cityId);
    }

    loadOnlinePlaces() {
        $.get("/Places/PlacesOnline", (data) => {
            $('#onlinePlaces').html(data);
        });
    }

    loadCountries() {
        const getCountriesUrl = $('#getCountriesUrl').val();
        $.getJSON(getCountriesUrl, (data) => {
            let items = '<option value="">Оберіть країну</option>';
            $.each(data, (i, country) => {
                items += `<option value="${country.id}">${country.name}</option>`;
            });
            $('#country').html(items);
        });
    }

    loadCities(countryId) {
        const getCitiesInCountryUrl = $('#getCitiesInCountryUrl').val();
        $.getJSON(getCitiesInCountryUrl, { countryId: countryId }, (data) => {
            let items = '<option value="">Оберіть місто</option>';
            $.each(data, (i, city) => {
                items += `<option value="${city.id}">${city.name}</option>`;
            });
            $('#city').html(items);
        });
    }

    loadPlacesInCity(cityId) {
        $.get(`/Places/PlacesInCity?cityId=${cityId}`, (data) => {
            $('#place').html(data);
        });
    }
}
$(document).ready(() => {
    const placeSelector = new PlaceSelector();
});