package com.arcane.arcana.user.service;

import com.arcane.arcana.common.entity.User;
import com.arcane.arcana.user.dto.RegisterDto;
import com.arcane.arcana.user.dto.UpdateDto;
import com.arcane.arcana.user.repository.UserRepository;
import com.arcane.arcana.common.util.JwtUtil;
import com.arcane.arcana.common.service.RedisService;
import com.arcane.arcana.common.exception.CustomException;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.http.HttpStatus;
import org.springframework.mail.javamail.JavaMailSender;
import org.springframework.mail.javamail.MimeMessageHelper;
import org.springframework.mail.javamail.MimeMessagePreparator;
import org.springframework.security.crypto.password.PasswordEncoder;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.util.Optional;
import java.util.Random;

/**
 * 사용자 서비스 클래스 구현
 */
@Service
public class UserServiceImpl implements UserService {

    private final UserRepository userRepository;
    private final PasswordEncoder passwordEncoder;
    private final JwtUtil jwtUtil;
    private final RedisService redisService;
    private final JavaMailSender mailSender;

    @Value("${spring.mail.username}")
    private String senderEmail;

    public UserServiceImpl(UserRepository userRepository, PasswordEncoder passwordEncoder,
        JwtUtil jwtUtil, RedisService redisService, JavaMailSender mailSender) {
        this.userRepository = userRepository;
        this.passwordEncoder = passwordEncoder;
        this.jwtUtil = jwtUtil;
        this.redisService = redisService;
        this.mailSender = mailSender;
    }

    @Override
    @Transactional
    public void registerUser(RegisterDto registerDto) {
        if (userRepository.existsByEmail(registerDto.getEmail())) {
            throw new CustomException("이미 존재하는 이메일입니다.", HttpStatus.BAD_REQUEST);
        }

        if (userRepository.existsByUsername(registerDto.getUsername())) {
            throw new CustomException("이미 존재하는 닉네임입니다.", HttpStatus.BAD_REQUEST);
        }

        User user = new User();
        user.setEmail(registerDto.getEmail());
        user.setUsername(registerDto.getUsername());
        user.encodePassword(registerDto.getPassword(), passwordEncoder);
        userRepository.save(user);

        String emailVerificationToken = jwtUtil.generateEmailVerificationToken(user.getEmail());
        String emailVerificationCode = generateVerificationCode();

        redisService.setStringValue("email_verification:" + user.getEmail(), emailVerificationToken,
            3600); // 1시간 유효
        redisService.setStringValue("email_code:" + user.getEmail(), emailVerificationCode, 3600);

        sendVerificationEmail(user.getEmail(), emailVerificationToken, emailVerificationCode);
    }

    private void sendVerificationEmail(String recipientEmail, String token, String code) {
        String verificationUrl =
            "http://localhost:8080/user/verify-email?email=" + recipientEmail + "&token=" + token;
        String subject = "이메일 인증";
        String content = "<p>안녕하세요!</p>"
            + "<p>Arcana 서비스에 가입해 주셔서 감사합니다.</p>"
            + "<p>아래 링크를 클릭하여 이메일 인증을 완료하거나, 인증번호를 입력해 주세요:</p>"
            + "<a href=\"" + verificationUrl + "\">이메일 인증하기</a>"
            + "<p>인증번호: <b>" + code + "</b></p>"
            + "<p>감사합니다.<br>Arcana 팀</p>";

        MimeMessagePreparator messagePreparator = mimeMessage -> {
            MimeMessageHelper helper = new MimeMessageHelper(mimeMessage, true, "UTF-8");
            helper.setFrom(senderEmail, "Arcana Team");
            helper.setTo(recipientEmail);
            helper.setSubject(subject);
            helper.setText(content, true);
        };

        mailSender.send(messagePreparator);
    }

    private String generateVerificationCode() {
        Random random = new Random();
        int code = 100000 + random.nextInt(900000); // 6자리 인증번호 생성
        return String.valueOf(code);
    }

    @Override
    public void verifyEmail(String email, String tokenOrCode) {
        if (tokenOrCode.length() == 6) {
            verifyEmailWithCode(email, tokenOrCode);
        } else {
            verifyEmailWithToken(email, tokenOrCode);
        }
    }

    private void verifyEmailWithToken(String email, String token) {
        String redisToken = redisService.getStringValue("email_verification:" + email);
        if (redisToken != null && redisToken.equals(token)) {
            Optional<User> userOpt = userRepository.findByEmail(email);
            if (userOpt.isPresent()) {
                User user = userOpt.get();
                user.setEmailVerified(true);
                userRepository.save(user);
                redisService.deleteValue("email_verification:" + email);
                redisService.deleteValue("email_code:" + email);
            } else {
                throw new CustomException("사용자를 찾을 수 없습니다.", HttpStatus.NOT_FOUND);
            }
        } else {
            throw new CustomException("유효하지 않은 토큰입니다.", HttpStatus.BAD_REQUEST);
        }
    }

    private void verifyEmailWithCode(String email, String code) {
        String redisCode = redisService.getStringValue("email_code:" + email);
        if (redisCode != null && redisCode.equals(code)) {
            Optional<User> userOpt = userRepository.findByEmail(email);
            if (userOpt.isPresent()) {
                User user = userOpt.get();
                user.setEmailVerified(true);
                userRepository.save(user);
                redisService.deleteValue("email_code:" + email);
                redisService.deleteValue("email_verification:" + email);
            } else {
                throw new CustomException("사용자를 찾을 수 없습니다.", HttpStatus.NOT_FOUND);
            }
        } else {
            throw new CustomException("유효하지 않은 인증번호입니다.", HttpStatus.BAD_REQUEST);
        }
    }

    @Override
    public String login(String email, String rawPassword) {
        User user = userRepository.findByEmail(email)
            .orElseThrow(
                () -> new CustomException("이메일 또는 비밀번호가 일치하지 않습니다.", HttpStatus.BAD_REQUEST));

        if (!user.isEmailVerified()) {
            throw new CustomException("이메일 인증이 완료되지 않았습니다.", HttpStatus.UNAUTHORIZED);
        }

        if (passwordEncoder.matches(rawPassword, user.getPassword())) {
            String accessToken = jwtUtil.generateAccessToken(user.getEmail());
            String refreshToken = jwtUtil.generateRefreshToken(user.getEmail());

            redisService.setStringValue("refresh_token:" + user.getEmail(), refreshToken,
                jwtUtil.getRefreshTokenExpirationMinutes());
            return accessToken;
        } else {
            throw new CustomException("이메일 또는 비밀번호가 일치하지 않습니다.", HttpStatus.BAD_REQUEST);
        }
    }

    @Override
    @Transactional
    public void updateUser(Long userId, UpdateDto updateDto) {
        User user = userRepository.findById(userId)
            .orElseThrow(() -> new CustomException("사용자를 찾을 수 없습니다.", HttpStatus.NOT_FOUND));

        if (updateDto.getOldPassword() != null && !passwordEncoder.matches(
            updateDto.getOldPassword(), user.getPassword())) {
            throw new CustomException("기존 비밀번호가 일치하지 않습니다.", HttpStatus.BAD_REQUEST);
        }

        if (updateDto.getUsername() != null && !updateDto.getUsername().isEmpty()) {
            if (userRepository.existsByUsername(updateDto.getUsername())) {
                throw new CustomException("이미 존재하는 닉네임입니다.", HttpStatus.BAD_REQUEST);
            }
            user.setUsername(updateDto.getUsername());
        }

        if (updateDto.getPassword() != null && !updateDto.getPassword().isEmpty()) {
            user.encodePassword(updateDto.getPassword(), passwordEncoder);
        }

        userRepository.save(user);
    }

    @Override
    public void logout(String email) {
        redisService.deleteValue("refresh_token:" + email);
    }

    @Override
    public Long getUserIdByEmail(String email) {
        return userRepository.findByEmail(email)
            .orElseThrow(() -> new CustomException("사용자를 찾을 수 없습니다.", HttpStatus.NOT_FOUND))
            .getId();
    }
}
