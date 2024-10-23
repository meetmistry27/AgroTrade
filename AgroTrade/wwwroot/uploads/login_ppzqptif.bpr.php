<?php

    require_once "config.php";
    session_start();
    $notlogin = false;
    $captcha=false;
    $_SESSION['login'] = false;
    if(isset($_POST['sbtn'])){
        $username = $_POST['username'];
        $password =$_POST['password'];
        $email = $_POST['email'];
        $sql = "SELECT `id` FROM `user` WHERE (`username`='$username' OR `email` = '$email' ) AND `password` = '$password' ";
        $result = mysqli_query($link,$sql);
        $password1=$_POST['captcha'];
    
// Evaluate the count
        if(mysqli_num_rows($result) > 0){
            if($password1===$_SESSION['CAPTCHA_CODE'])
            {
            while($row = mysqli_fetch_array($result)){
                $id = $row['id'];
            }
            $_SESSION['login'] = true;
            $_SESSION['username'] = $username;
            $_SESSION['password']=$password;
            $_SESSION['id']=$id;
            }
            else
            {
                $captcha=true;
            }
        }
        else{
            $notlogin = true;
        }
    }
?>
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Document</title>
</head>
<body>
<form action="" method = "post">
        <h1>LOGIN</h1>
        <br>
        <label for="username">Username</label>
        <input type="text" name="username" id="username" placeholder="Enter your Email here" required></input>
        <label for="Password">Password:</label>
        <input type="text" name="password" id="password" placeholder="Enter your password" required></input>
        <label for="Email">Email:</label>
        <input type="email" placeholder = "Type your email here" name = "email" required></input>
        <label for="captcha">Enter Captcha:</label>
        <input type="text" name="captcha" id="captcha" required><br>
        <img src="captcha.php" alt="Captcha Image"><br><br>
        
        <input type="submit" name = "sbtn"></input>
        <?php
        if($_SESSION['login'] == true){
            header("location:myaccount.php");
        }
        if($notlogin){
            echo "incorrect username or password";
        }
        if($captcha)
        {
            echo "incorrect captcha";
        }
        ?>
    </form>
</body>
</html>