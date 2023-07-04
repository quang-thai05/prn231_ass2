var token = localStorage.getItem("token");
var decoded = jwt_decode(token);
var userId = decoded["UserId"];
var orderDetails = [];

/* Set values + misc */
var promoCode;
var promoPrice;
var fadeTime = 300;

$(document).ready(function () {
	getCart();
});

function removeCartItem(model) {
	removeItem(model);
	var productId = model.getAttribute("data-product-id");
	$.ajax({
		url: "https://localhost:7135/api/Cart/RemoveCartItem/" + productId,
		method: "DELETE",
		headers: {
			Authorization: "Bearer " + token,
		},
		contentType: "application/json",
		success: (response) => {
			console.log("removed successfully!");
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

function getCart() {
	$.ajax({
		url: "https://localhost:7135/api/Cart/GetCartItems",
		method: "GET",
		contentType: "application/json",
		headers: {
			Authorization: "Bearer " + token,
		},
		success: (response) => {
			response.map((item) =>
				orderDetails.push({
					productId: item.productId,
					quantity: item.quantity,
					discount: 0,
				})
			);

			$(".basket").append(
				response.map(
					(item) =>
						`<div class="basket-product">
							<div class="item">
								<div class="product-image">
									<img src="http://placehold.it/120x166" alt="Placholder Image 2" class="product-frame">
								</div>	
								<div class="product-details">
									<h1>
										<strong>
											<span class="item-quantity">${item.quantity}</span> x
										</strong>
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
									value="${item.quantity}"
									onchange="updateQuantity(this)"
								/>
							</div>
							<div class="subtotal">${item.unitPrice * item.quantity}</div>
							<div class="remove">
								<button 
									data-product-id="${item.productId}" 
									onclick="removeCartItem(this)"
								>
									Remove
								</button>
							</div>
						</div>`
				)
			);
			updateSumItems();
			recalculateCart();
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

	//If there is a valid promoCode, and subtotal < 10 subtract from total
	var promoPrice = parseFloat($(".promo-value").text());
	if (promoPrice) {
		if (subtotal >= 10) {
			total -= promoPrice;
		} else {
			alert("Order must be more than £10 for Promo code to apply.");
			$(".summary-promo").addClass("hide");
		}
	}

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

function makeOrder() {
	let currentDate = new Date();
	var tomorrowDate = new Date(currentDate.getTime() + 24 * 60 * 60 * 1000);
	let threeDaysLater = new Date(
		currentDate.getTime() + 3 * 24 * 60 * 60 * 1000
	);

	let order = {
		orderDate: currentDate,
		requiredDate: threeDaysLater,
		shippedDate: tomorrowDate,
		Freight: 1,
		orderDetails: orderDetails,
	};

	$.ajax({
		url: "https://localhost:7135/api/Order/AddOrder/" + userId,
		method: "POST",
		contentType: "application/json",
		data: JSON.stringify(order),
		headers: {
			Authorization: "Bearer " + token,
		},
		success: (response) => {
			SlimNotifierJs.notification("success", "Error", response, 3000);
			setTimeout(() => {window.location.href = "./index.html"}, 3000);
		},
		error: (xhr) => {
			SlimNotifierJs.notification(
				"error",
				"Error",
				xhr.responseText,
				3000
			);
		},
	});
}

function removeAllItem() {
	$.ajax({
		url: "https://localhost:7135/api/Cart/RemoveAllItems",
		method: "DELETE",
		contentType: "application/json",
		headers: {
			Authorization: "Bearer " + token,
		},
		success: (response) => {
			console.log("Removed All Items");
		},
		error: (xhr) => {
			SlimNotifierJs.notification(
				"error",
				"Error",
				xhr.responseText,
				3000
			);
		},
	});
}
