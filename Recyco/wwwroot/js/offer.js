

//async function getAllOffers(offers) {
//    try {
//        const result = await fetch("http://localhost:5289/Offers" + offers).then(result => result.json()).then(result => result.data)
//        if (!response.ok) {
//            throw new Error(`HTTP error! Status: ${response.status}`).then(result => result.json()).then(result => result.data);
//        }
//        const data = await response.json();
//        return data.data; // Assuming the API returns an object with a 'data' property
//    } catch (error) {
//        console.error('Error fetching all offers:', error);
//        return []; // Return an empty array in case of an error
//    }
//}






async function getOffersByCategory(wasteType) {
    try {
        const result = await fetch("http://localhost:5289/Offers/OffersByCategory/?offerCategory=" + wasteType).then(result => result.json()).then(result => result.data)
        console.log(result, "**************************************************")
        return result

    } catch (error) {
        const result = await fetch("http://localhost:5289/Offers/OffersByCategory/?offerCategory=all").then(result => result.json()).then(result => result.data)
        return result
    }
}

function createElementFromHTML(htmlString) {
    var div = document.createElement('div');
    div.innerHTML = htmlString.trim();

    // Change this to div.childNodes to support multiple top-level nodes.
    return div.firstChild;
}

const wasteTypeToImageMap = {
    'plastic': '../img/Plastic.jpeg',
    'metal': '../img/Metale.jpeg',
    'paper': '../img/Paper.jpeg',
    'concrete': '../img/Concrete.jpg',
    'hazardous': '../img/hazardous.jpg',
    'wood': '../img/Wood.jpeg',
};

async function wasteTypeFilter(wasteType) {
    data = await getOffersByCategory(wasteType)
    element = document.getElementById("offers")
    console.log("etetsetste", wasteType)
    element.innerHTML = ""
    console.log(element)
    data.forEach(offer => {
        const imageUrl = wasteTypeToImageMap[wasteType] || '../img/user.png';
        console.log(offer.waste.type, "wrerzr type")// Fallback to a default image
        const htmlString = `
            <div class="card shadow mb-3" style="width: 18rem;">
                <div class="card-header">
                    <img src="${imageUrl}" style="height:20vh;" />
                </div>
            




                 <div class="card-body">

                            <a class="card-title text-decoration-underline h4 mb-3" asp-action="Details" asp-route-id="@item.OfferId" class="text-light fw-bolder">
                                ${offer.waste.title}
                            </a>
                            <h6 class="card-subtitle mt-2 mb-2 text-muted">
                                Type: ${wasteType}
                            </h6>
                            <dl class=" d-felx flex-column ps-3 pt-3 ">
                                <dt class=" row ">
                                    Description :
                                </dt>
                                <dd class="row  ">
                                    ${offer.waste.description}
                                </dd>
                                <dt class="row">
                                    Start Price :
                                </dt>
                                <dd class="row fw-bold text-danger">
                                   $ ${offer.startPrice} 
                                </dd>
                                <dt class="row">
                                    Start Date :
                                </dt>
                                <dd class="row">
                                   ${offer.createdAt}
                                </dd>
                                <dt class="row">
                                    End Date :
                                </dt>
                                <dd class="row">
                                    ${offer.endDate}
                                </dd>
                            </dl>
                 </div>
                 <a href="/Offers/Details/${offer.offerId}" class="text-info fw-bold p-3""${offer.offerId}">View Offer</a>


            </div>`;
        const offerElement = createElementFromHTML(htmlString);
        element.appendChild(offerElement);
    });
}