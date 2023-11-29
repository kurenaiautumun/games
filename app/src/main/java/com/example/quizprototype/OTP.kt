package com.example.quizprototype

import android.content.Intent
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.text.Editable
import android.widget.Button
import android.widget.EditText
import android.widget.TextView
import android.widget.Toast
import at.favre.lib.crypto.bcrypt.BCrypt
import com.google.firebase.auth.FirebaseAuth
import com.google.firebase.auth.PhoneAuthCredential
import com.google.firebase.auth.PhoneAuthProvider
import com.google.firebase.database.FirebaseDatabase

class OTP : AppCompatActivity() {
    private lateinit var otp : EditText
    private lateinit var verify : Button
    private lateinit var auth : FirebaseAuth
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_otp)

        val number = intent.getStringExtra("phone")
        val numbertext = findViewById<TextView>(R.id.initial_number)
        numbertext.text = "+91"+ number

        val verificaton = intent.getStringExtra("verification")
        otp = findViewById(R.id.OTPnumber)
        auth = FirebaseAuth.getInstance()
        verify = findViewById(R.id.verificationbutton)

        verify.setOnClickListener {
            verifyPhoneNumberWithCode(verificaton.toString(),otp.text.toString())
        }
    }

    fun verifyPhoneNumberWithCode(verificationId: String, code: String) {
        val credential = PhoneAuthProvider.getCredential(verificationId, code)
        signInWithPhoneAuthCredential(credential)
    }

    // Function to sign in with the obtained credentials
    private fun signInWithPhoneAuthCredential(credential: PhoneAuthCredential) {
        auth.signInWithCredential(credential)
            .addOnCompleteListener(this) { task ->
                if (task.isSuccessful) {
                    // Verification successful, handle sign in
                    val databaseReference = FirebaseDatabase.getInstance().reference
                    val username = intent.getStringExtra("user")
                    val password = intent.getStringExtra("password")
                    val phoneNumber = intent.getStringExtra("phone")  // Replace with the actual phone number
                    val uid = auth.currentUser?.uid  // Replace with the actual UID

                    val hashedPassword = hashPassword(password.toString())

                    val userReference = databaseReference.child("users").child(phoneNumber.toString())
                    userReference.child("password").setValue(hashedPassword)
                    userReference.child("user").setValue(username)
                    userReference.child("uid").setValue(uid)

                    startActivity(Intent(this,Login::class.java))
                    finish()
                } else {
                    Toast.makeText(this@OTP,"Invalid OTP",Toast.LENGTH_LONG).show()
                }
            }
    }

    fun hashPassword(password: String): String {
        return BCrypt.withDefaults().hashToString(12, password.toCharArray())
    }

}