document.addEventListener('DOMContentLoaded', function () {
    fetch('/admin/geo/GetAllCountries')
        .then(response => response.json())
        .then(data => {
            const dropdown = document.getElementById('countryDropdown');
            dropdown.innerHTML = '<option value="">Select a country</option>';
            data.forEach(country => {
                const option = `<option value="${country.id}">${country.name}</option>`;
                dropdown.innerHTML += option;
            });
        })
        .catch(error => {
            console.error('Error fetching countries:', error);
        });
});