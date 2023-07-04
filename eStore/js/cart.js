var token = localStorage.getItem("token");

/* Set values + misc */
var promoCode;
var promoPrice;
var fadeTime = 300;

/* Assign actions */
$(".quantity input").change(function () {
	updateQuantity(this);
});

$(".remove button").click(function () {
	removeItem(this);
	$.ajax({
		url: "https://localhost:7135/api/Cart/RemoveFromCart",
		method: "DELETE",
		contentType: "application/json",
		success: (response) => {
			console.log("removed successfully!");
		},
		error: function (xhr, status, error) {
			SlimNotifierJs.notification(
				"error",
				"Error",
				xhr.responseText,
				3000
			);
		},
	});
});

$(document).ready(function () {
	updateSumItems();
	getCart();
});

var cookies = document.cookie

function getCart() {
	$.ajax({
		url: "https://localhost:7135/api/Cart/GetCart",
		method: "GET",
		xhrFields: {
			withCredentials: true
		},
		contentType: "application/json",
		headers: {
			Authorization: "Bearer " + token
		},
		success: (response) => {
			$(".basket").append(
				response.items.map(
					(item) =>
						`<div class="basket-product">
                        <div class="item">
                            <div class="product-details">
                                <h1>
                                    <strong><span class="item-quantity">${item.quantity}</span> x</strong>
                                    ${item.productName}
                                </h1>
                            </div>
                        </div>
                        <div class="price">${item.unitPrice}</div>
                        <div class="quantity">
                            <input
                                type="number"
                                min="1"
                                class="quantity-field"
                            />
                        </div>
                        <div class="subtotal"></div>
                        <div class="remove">
                            <button>Remove</button>
                        </div>
                    </div>`
				)
			);
		},
		error: function (xhr) {
			SlimNotifierJs.notification(
				"error",
				"Error",
				xhr.responseText,
				3000
			);
		},
	});
}

/* Recalculate cart */
function recalculateCart(onlyTotal) {
	var subtotal = 0;

	/* Sum up row totals */
	$(".basket-product").each(function () {
		subtotal += parseFloat($(this).children(".subtotal").text());
	});

	/* Calculate totals */
	var total = subtotal;

	/*If switch for update only total, update only total display*/
	if (onlyTotal) {
		/* Update total display */
		$(".total-value").fadeOut(fadeTime, function () {
			$("#basket-total").html(total.toFixed(2));
			$(".total-value").fadeIn(fadeTime);
		});
	} else {
		/* Update summary display. */
		$(".final-value").fadeOut(fadeTime, function () {
			$("#basket-subtotal").html(subtotal.toFixed(2));
			$("#basket-total").html(total.toFixed(2));
			if (total == 0) {
				$(".checkout-cta").fadeOut(fadeTime);
			} else {
				$(".checkout-cta").fadeIn(fadeTime);
			}
			$(".final-value").fadeIn(fadeTime);
		});
	}
}

/* Update quantity */
function updateQuantity(quantityInput) {
	/* Calculate line price */
	var productRow = $(quantityInput).parent().parent();
	var price = parseFloat(productRow.children(".price").text());
	var quantity = $(quantityInput).val();
	var linePrice = price * quantity;

	/* Update line price display and recalc cart totals */
	productRow.children(".subtotal").each(function () {
		$(this).fadeOut(fadeTime, function () {
			$(this).text(linePrice.toFixed(2));
			recalculateCart();
			$(this).fadeIn(fadeTime);
		});
	});

	productRow.find(".item-quantity").text(quantity);
	updateSumItems();
}

function updateSumItems() {
	var sumItems = 0;
	$(".quantity input").each(function () {
		sumItems += parseInt($(this).val());
	});
	$(".total-items").text(sumItems);
}

/* Remove item from cart */
function removeItem(removeButton) {
	/* Remove row from DOM and recalc cart total */
	var productRow = $(removeButton).parent().parent();
	productRow.slideUp(fadeTime, function () {
		productRow.remove();
		recalculateCart();
		updateSumItems();
	});
}
