package com.example.quizprototype

import android.content.Context
import android.content.Intent
import android.content.SharedPreferences
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.util.Log
import android.widget.Button
import android.widget.EditText
import android.widget.TextView
import android.widget.Toast
import at.favre.lib.crypto.bcrypt.BCrypt
import com.google.android.gms.ads.MobileAds
import com.google.firebase.database.*

class Login : AppCompatActivity() {
    private  lateinit var mobilNo:EditText
    private lateinit var password: EditText
    private lateinit var sharedPreferences: SharedPreferences
    override fun onCreate(savedInstanceState: Bundle?) {

        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_login)

        val login = findViewById<Button>(R.id.login)
        mobilNo = findViewById(R.id.lognumber)
        password = findViewById(R.id.logpassword)


        login.setOnClickListener {
            val tempphone = mobilNo.text
            val temppass = password.text
            signInWithPhoneNumberAndPassword(tempphone.toString(),temppass.toString())
        }


        val newacc = findViewById<TextView>(R.id.newacc)
        newacc.setOnClickListener {
            val intent = Intent(this,Signup::class.java)
            startActivity(intent)
            finish()
        }
    }

    fun signInWithPhoneNumberAndPassword(phoneNumber: String, password: String) {
        // Assuming you have a reference to your Firebase database
        sharedPreferences = getSharedPreferences("MyPrefs", Context.MODE_PRIVATE)
        val databaseReference = FirebaseDatabase.getInstance().reference

        // Retrieve the hashed password from the database using the phone number
        val userReference = databaseReference.child("users").child(phoneNumber)
        userReference.addListenerForSingleValueEvent(object : ValueEventListener {
            override fun onDataChange(dataSnapshot: DataSnapshot) {
                val storedHashedPassword = dataSnapshot.child("password").getValue(String::class.java)

                if (storedHashedPassword != null && validatePassword(password, storedHashedPassword)) {
                    // Password is correct, generate a custom token on the server
                    val tempuid = dataSnapshot.child("uid").getValue(String::class.java)
                    val tempname = dataSnapshot.child("user").getValue(String::class.java)
                    val editor = sharedPreferences.edit()
                    editor.putString("UID",tempuid)
                    editor.putString("name",tempname)
                    editor.apply()

                    val intent = Intent(this@Login,BaseActivity::class.java)
                    startActivity(intent)
                } else {
                    // Incorrect password
                    // Handle the error or notify the user
                    Toast.makeText(this@Login,"Incorrect Password",Toast.LENGTH_LONG).show()
                }
            }

            override fun onCancelled(databaseError: DatabaseError) {
                // Handle errors
                // Notify the user about the error
                Log.d("ERR",databaseError.toString())
            }
        })
    }

    fun validatePassword(inputPassword: String, hashedPassword: String): Boolean {
        val result = BCrypt.verifyer().verify(inputPassword.toCharArray(), hashedPassword)
        return result.verified
    }
}