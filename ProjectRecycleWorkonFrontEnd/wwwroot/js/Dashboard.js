const body = document.querySelector("body"),
    sidebar = body.querySelector("nav"),
    sidebarToggle = body.querySelector(".sidebar-toggle");


sidebarToggle.addEventListener("click", () => {
    sidebar.classList.toggle("close")
});

/* Demo purposes only */
$(".hover").mouseleave(
    function () {
        $(this).removeClass("hover");
    }
);
