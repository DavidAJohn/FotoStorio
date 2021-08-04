function setupStripe(publishable_key) {

    let stripe = Stripe(publishable_key);

    let elements = stripe.elements();

    let style = {
        base: {
            color: "#111750",
            fontFamily: 'Roboto, Arial sans-serif',
            fontSmoothing: "antialiased",
            fontSize: "16px",
            "::placeholder": {
            color: "#111750"
            }
        },
        invalid: {
            fontFamily: 'Roboto, Arial, sans-serif',
            color: "#FF2A57",
            iconColor: "#FF2A57"
        }
    };

    let card = elements.create('card', { style: style });

    mountCard(card);

    card.on('change', function (event) {
        document.querySelector("#btnPlaceOrder").disabled = event.empty;
        document.querySelector("#card-error").textContent = event.error ? event.error.message : "";
    });
};

const mountCard = (card) => {
    let checkExists = setInterval(function() {
        if (document.querySelector("#card-element") !== null) {
            card.mount("#card-element")
            clearInterval(checkExists);
        }
     }, 100);
};
