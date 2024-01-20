package com.example.quizprototype

import android.content.Intent
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.text.Editable
import android.util.Log
import android.widget.Button
import android.widget.EditText
import android.widget.TextView
import android.widget.Toast
import com.example.quizprototype.Response_Bodies.savedata_response
import com.example.quizprototype.Retrofit_Object_and_Interface.Retrofitobject
import com.google.firebase.FirebaseException
import com.google.firebase.auth.FirebaseAuth
import com.google.firebase.auth.PhoneAuthCredential
import com.google.firebase.auth.PhoneAuthOptions
import com.google.firebase.auth.PhoneAuthProvider
import com.google.firebase.database.DataSnapshot
import com.google.firebase.database.DatabaseError
import com.google.firebase.database.FirebaseDatabase
import com.google.firebase.database.ValueEventListener
import okhttp3.MultipartBody
import retrofit2.Call
import retrofit2.Callback
import retrofit2.Response
import java.util.concurrent.TimeUnit

class Signup : AppCompatActivity() {
    private  lateinit var mauth: FirebaseAuth
    private lateinit var mobileNo : EditText
    private lateinit var uname :EditText
    private lateinit var password :EditText
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_signup)


        val already = findViewById<TextView>(R.id.Already)
        val register = findViewById<Button>(R.id.buttonreg)
        uname = findViewById(R.id.usernamereg)
        password = findViewById(R.id.Passwordreg)
        mobileNo = findViewById(R.id.regPhone)
        val number = mobileNo.text
        mauth = FirebaseAuth.getInstance()



        register.setOnClickListener {
            checkAlready(number.toString(), object : CheckAlreadyCallback {
                override fun onResult(isAlreadyExist: Boolean) {
                    if (isAlreadyExist) {
                        Toast.makeText(this@Signup, "Number Already Exist", Toast.LENGTH_LONG).show()
                    } else {
                        if (uname.text.isNotEmpty() && password.text.isNotEmpty() && mobileNo.text.isNotEmpty()) {
                            if (number.length == 10) {
                                startverification()
                            } else {
                                Toast.makeText(this@Signup, "Invalid Phone Number", Toast.LENGTH_LONG).show()
                            }
                        } else {
                            Toast.makeText(this@Signup, "Details not filled", Toast.LENGTH_LONG).show()
                        }
                    }
                }
            })
        }

        already.setOnClickListener {
            startActivity(Intent(this,Login::class.java))
            finish()
        }
    }

    interface CheckAlreadyCallback {
        fun onResult(isAlreadyExist: Boolean)
    }

    private fun checkAlready(number: String, callback: CheckAlreadyCallback) {
        val databaseReference = FirebaseDatabase.getInstance().reference
        val userReference = databaseReference.child("users")

        userReference.child(number).addListenerForSingleValueEvent(object : ValueEventListener {
            override fun onDataChange(dataSnapshot: DataSnapshot) {
                callback.onResult(dataSnapshot.exists())
            }

            override fun onCancelled(databaseError: DatabaseError) {
                // Handle errors
                // Notify the user about the error
                Log.d("ERR", databaseError.toString())
                callback.onResult(false)  // Assume it doesn't exist in case of an error
            }
        })
    }

    private fun startverification() {
        val callbacks = object : PhoneAuthProvider.OnVerificationStateChangedCallbacks() {
            override fun onVerificationCompleted(credential: PhoneAuthCredential) {

            }

            override fun onVerificationFailed(e: FirebaseException) {
                Toast.makeText(this@Signup,e.localizedMessage.toString(), Toast.LENGTH_LONG).show()
                Log.d("resp",e.localizedMessage.toString())
            }

            override fun onCodeSent(
                verificationId: String,
                token: PhoneAuthProvider.ForceResendingToken
            ) {


                val tempuser = uname.text
                val temppass = password.text
                val tempnum = mobileNo.text
                data_save_backend(tempuser,tempnum)
                val intent = Intent(this@Signup,OTP::class.java)
                intent.putExtra("verification",verificationId)
                intent.putExtra("user",tempuser.toString())
                intent.putExtra("password",temppass.toString())
                intent.putExtra("phone",tempnum.toString())
                startActivity(intent)
            }
        }

        val options = PhoneAuthOptions.newBuilder(mauth)
            .setPhoneNumber("+91"+mobileNo.text.toString().trim())       // Phone number to verify
            .setTimeout(60L, TimeUnit.SECONDS) // Timeout duration
            .setActivity(this)                 // Activity (for callback binding)
            .setCallbacks(callbacks)            // OnVerificationStateChangedCallbacks
            .build()

        PhoneAuthProvider.verifyPhoneNumber(options)
    }

    private fun data_save_backend(tempuser: Editable, tempnum: Editable) {
        val requestBody = MultipartBody.Builder()
            .setType(MultipartBody.FORM)
            .addFormDataPart("number", tempnum.toString())
            .addFormDataPart("name",tempuser.toString())
            .build()

        Retrofitobject.apiInterface.saveData(requestBody).enqueue(object : Callback<savedata_response?> {
            override fun onResponse(
                call: Call<savedata_response?>,
                response: Response<savedata_response?>
            ) {

            }
            override fun onFailure(call: Call<savedata_response?>, t: Throwable) {
               Toast.makeText(this@Signup, call.toString(),Toast.LENGTH_LONG).show()
            }
        })
    }
}